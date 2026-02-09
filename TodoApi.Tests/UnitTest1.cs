using Xunit;
using Moq;
using TodoApi.Services;
using TodoApi.Models;
using TodoApi.DTOs;
using TodoApi.Controllers;
using TodoApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TodoApi.Tests;

public class TodoServiceTests
{
    [Fact]
    public async Task CreateTodoAsync_ShouldReturnTodoDto()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        var createDto = new CreateTodoDto
        {
            Title = "Test TODO",
            Description = "Test Description",
            IsCompleted = false
        };

        var expectedTodo = new Todo
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            IsCompleted = createDto.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        mockRepository.Setup(r => r.CreateAsync(It.IsAny<Todo>()))
            .ReturnsAsync(expectedTodo);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.CreateTodoAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(expectedTodo.Id, result.Id);
        Assert.Equal(expectedTodo.Title, result.Title);
        Assert.Equal(expectedTodo.Description, result.Description);
        Assert.Equal(expectedTodo.IsCompleted, result.IsCompleted);
        mockRepository.Verify(r => r.CreateAsync(It.IsAny<Todo>()), Times.Once);
    }

    [Fact]
    public async Task GetAllTodosAsync_ShouldReturnListOfTodos()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        var todos = new List<Todo>
        {
            new Todo { Id = 1, Title = "Todo 1", Description = "Desc 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new Todo { Id = 2, Title = "Todo 2", Description = "Desc 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };

        mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(todos);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.GetAllTodosAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllTodosAsync_WhenNoTodos_ShouldReturnEmptyList()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Todo>());

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.GetAllTodosAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WhenExists_ShouldReturnTodo()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        var todo = new Todo 
        { 
            Id = 1, 
            Title = "Test", 
            Description = "Test Desc", 
            IsCompleted = false, 
            CreatedAt = DateTime.UtcNow 
        };

        mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.GetTodoByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Title);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Todo?)null);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.GetTodoByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenExists_ShouldReturnUpdatedTodo()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        var updateDto = new UpdateTodoDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = true
        };

        var updatedTodo = new Todo
        {
            Id = 1,
            Title = updateDto.Title,
            Description = updateDto.Description,
            IsCompleted = updateDto.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        mockRepository.Setup(r => r.UpdateAsync(1, It.IsAny<Todo>()))
            .ReturnsAsync(updatedTodo);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.UpdateTodoAsync(1, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Description", result.Description);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenNotExists_ShouldReturnNull()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        var updateDto = new UpdateTodoDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = true
        };

        mockRepository.Setup(r => r.UpdateAsync(999, It.IsAny<Todo>()))
            .ReturnsAsync((Todo?)null);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.UpdateTodoAsync(999, updateDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTodoAsync_WhenExists_ShouldReturnTrue()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.DeleteTodoAsync(1);

        Assert.True(result);
        mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTodoAsync_WhenNotExists_ShouldReturnFalse()
    {
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        var result = await service.DeleteTodoAsync(999);

        Assert.False(result);
    }
}

public class TodosControllerTests
{
    [Fact]
    public async Task CreateTodo_WithValidData_ReturnsCreatedResult()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        var createDto = new CreateTodoDto
        {
            Title = "Test TODO",
            Description = "Test Description"
        };

        var todoDto = new TodoDto
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        mockService.Setup(s => s.CreateTodoAsync(createDto)).ReturnsAsync(todoDto);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.CreateTodo(createDto);

        var actionResult = Assert.IsType<ActionResult<TodoDto>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnValue = Assert.IsType<TodoDto>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsOkResultWithTodos()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        var todos = new List<TodoDto>
        {
            new TodoDto { Id = 1, Title = "Todo 1", CreatedAt = DateTime.UtcNow },
            new TodoDto { Id = 2, Title = "Todo 2", CreatedAt = DateTime.UtcNow }
        };

        mockService.Setup(s => s.GetAllTodosAsync()).ReturnsAsync(todos);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.GetAllTodos();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<TodoDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<TodoDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetAllTodos_WhenNoTodos_ReturnsEmptyList()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        mockService.Setup(s => s.GetAllTodosAsync()).ReturnsAsync(new List<TodoDto>());

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.GetAllTodos();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<TodoDto>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<TodoDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetTodoById_WhenExists_ReturnsOkResult()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        var todoDto = new TodoDto
        {
            Id = 1,
            Title = "Test TODO",
            CreatedAt = DateTime.UtcNow
        };

        mockService.Setup(s => s.GetTodoByIdAsync(1)).ReturnsAsync(todoDto);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.GetTodoById(1);

        var actionResult = Assert.IsType<ActionResult<TodoDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsType<TodoDto>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetTodoById_WhenNotExists_ReturnsNotFound()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        mockService.Setup(s => s.GetTodoByIdAsync(999)).ReturnsAsync((TodoDto?)null);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.GetTodoById(999);

        var actionResult = Assert.IsType<ActionResult<TodoDto>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ReturnsOkResult()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        var updateDto = new UpdateTodoDto
        {
            Title = "Updated TODO",
            Description = "Updated Description",
            IsCompleted = true
        };

        var todoDto = new TodoDto
        {
            Id = 1,
            Title = updateDto.Title,
            Description = updateDto.Description,
            IsCompleted = updateDto.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        mockService.Setup(s => s.UpdateTodoAsync(1, updateDto)).ReturnsAsync(todoDto);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.UpdateTodo(1, updateDto);

        var actionResult = Assert.IsType<ActionResult<TodoDto>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsType<TodoDto>(okResult.Value);
        Assert.Equal("Updated TODO", returnValue.Title);
        Assert.True(returnValue.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodo_WhenNotExists_ReturnsNotFound()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        var updateDto = new UpdateTodoDto
        {
            Title = "Updated TODO",
            Description = "Updated Description",
            IsCompleted = true
        };

        mockService.Setup(s => s.UpdateTodoAsync(999, updateDto)).ReturnsAsync((TodoDto?)null);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.UpdateTodo(999, updateDto);

        var actionResult = Assert.IsType<ActionResult<TodoDto>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WhenExists_ReturnsNoContent()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        mockService.Setup(s => s.DeleteTodoAsync(1)).ReturnsAsync(true);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.DeleteTodo(1);

        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WhenNotExists_ReturnsNotFound()
    {
        var mockService = new Mock<ITodoService>();
        var mockLogger = new Mock<ILogger<TodosController>>();
        
        mockService.Setup(s => s.DeleteTodoAsync(999)).ReturnsAsync(false);

        var controller = new TodosController(mockService.Object, mockLogger.Object);

        var result = await controller.DeleteTodo(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
