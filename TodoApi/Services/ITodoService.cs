using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Services
{
    /// <summary>
    /// Service interface for TODO business logic operations
    /// Provides abstraction over service implementation
    /// </summary>
    public interface ITodoService
    {
        /// <summary>
        /// Creates a new TODO item
        /// </summary>
        Task<TodoDto> CreateTodoAsync(CreateTodoDto createDto);

        /// <summary>
        /// Retrieves all TODO items
        /// </summary>
        Task<IEnumerable<TodoDto>> GetAllTodosAsync();

        /// <summary>
        /// Retrieves a specific TODO item by its ID
        /// </summary>
        Task<TodoDto?> GetTodoByIdAsync(int id);

        /// <summary>
        /// Updates an existing TODO item
        /// </summary>
        Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateDto);

        /// <summary>
        /// Deletes a TODO item by its ID
        /// </summary>
        Task<bool> DeleteTodoAsync(int id);
    }
}
