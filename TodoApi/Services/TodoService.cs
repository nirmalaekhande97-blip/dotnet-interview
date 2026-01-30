using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    /// <summary>
    /// Service implementation for TODO business logic
    /// Orchestrates between controllers and repositories
    /// </summary>
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ILogger<TodoService> _logger;

        public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
        {
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TodoDto> CreateTodoAsync(CreateTodoDto createDto)
        {
            _logger.LogInformation("Creating new TODO item with title: {Title}", createDto.Title);

            var todo = new Todo
            {
                Title = createDto.Title,
                Description = createDto.Description,
                IsCompleted = createDto.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            var createdTodo = await _todoRepository.CreateAsync(todo);
            return MapToDto(createdTodo);
        }

        public async Task<IEnumerable<TodoDto>> GetAllTodosAsync()
        {
            _logger.LogInformation("Retrieving all TODO items");
            var todos = await _todoRepository.GetAllAsync();
            return todos.Select(MapToDto);
        }

        public async Task<TodoDto?> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving TODO item with ID: {TodoId}", id);
            var todo = await _todoRepository.GetByIdAsync(id);
            return todo != null ? MapToDto(todo) : null;
        }

        public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateDto)
        {
            _logger.LogInformation("Updating TODO item with ID: {TodoId}", id);

            var todo = new Todo
            {
                Title = updateDto.Title,
                Description = updateDto.Description,
                IsCompleted = updateDto.IsCompleted
            };

            var updatedTodo = await _todoRepository.UpdateAsync(id, todo);
            return updatedTodo != null ? MapToDto(updatedTodo) : null;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            _logger.LogInformation("Deleting TODO item with ID: {TodoId}", id);
            return await _todoRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Maps a Todo domain model to a TodoDto
        /// </summary>
        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt
            };
        }
    }
}
