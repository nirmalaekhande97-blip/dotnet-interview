using TodoApi.DTOs;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        Task<TodoDto> CreateTodoAsync(CreateTodoDto createDto);
        Task<IEnumerable<TodoDto>> GetAllTodosAsync();
        Task<TodoDto?> GetTodoByIdAsync(int id);
        Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto updateDto);
        Task<bool> DeleteTodoAsync(int id);
    }
}
