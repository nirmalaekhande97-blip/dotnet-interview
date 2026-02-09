using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TodoApi.Configuration;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TodoRepository> _logger;

        public TodoRepository(IOptions<DatabaseOptions> options, ILogger<TodoRepository> logger)
        {
            _connectionString = options.Value.ConnectionString 
                ?? throw new ArgumentNullException(nameof(options), "Connection string cannot be null");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                    VALUES (@title, @description, @isCompleted, @createdAt);
                    SELECT last_insert_rowid();
                ";

                command.Parameters.AddWithValue("@title", todo.Title);
                command.Parameters.AddWithValue("@description", todo.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);
                command.Parameters.AddWithValue("@createdAt", todo.CreatedAt.ToString("o"));

                var id = Convert.ToInt32(await command.ExecuteScalarAsync());
                todo.Id = id;

                _logger.LogInformation("Created TODO item with ID {TodoId}", id);
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TODO item");
                throw;
            }
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            try
            {
                var todos = new List<Todo>();
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos ORDER BY CreatedAt DESC";

                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    todos.Add(MapReaderToTodo(reader));
                }

                _logger.LogInformation("Retrieved {Count} TODO items", todos.Count);
                return todos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all TODO items");
                throw;
            }
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var todo = MapReaderToTodo(reader);
                    _logger.LogInformation("Retrieved TODO item with ID {TodoId}", id);
                    return todo;
                }

                _logger.LogWarning("TODO item with ID {TodoId} not found", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving TODO item with ID {TodoId}", id);
                throw;
            }
        }

        public async Task<Todo?> UpdateAsync(int id, Todo todo)
        {
            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE Todos
                    SET Title = @title, Description = @description, IsCompleted = @isCompleted
                    WHERE Id = @id
                ";

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", todo.Title);
                command.Parameters.AddWithValue("@description", todo.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCompleted", todo.IsCompleted ? 1 : 0);

                var rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected == 0)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found for update", id);
                    return null;
                }

                todo.Id = id;
                _logger.LogInformation("Updated TODO item with ID {TodoId}", id);
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TODO item with ID {TodoId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Todos WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                var deleted = rowsAffected > 0;

                if (deleted)
                {
                    _logger.LogInformation("Deleted TODO item with ID {TodoId}", id);
                }
                else
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found for deletion", id);
                }

                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting TODO item with ID {TodoId}", id);
                throw;
            }
        }

        private static Todo MapReaderToTodo(SqliteDataReader reader)
        {
            return new Todo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) 
                    ? null 
                    : reader.GetString(reader.GetOrdinal("Description")),
                IsCompleted = reader.GetInt32(reader.GetOrdinal("IsCompleted")) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
            };
        }
    }
}
