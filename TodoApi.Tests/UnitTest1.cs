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

/// <summary>
/// Unit tests for the TODO API
/// These tests use mocking to isolate the units under test
/// </summary>
public class TodoServiceTests
{
    [Fact]
    public async Task CreateTodoAsync_ShouldReturnTodoDto()
    {
        // Arrange
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

        // Act
        var result = await service.CreateTodoAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTodo.Id, result.Id);
        Assert.Equal(expectedTodo.Title, result.Title);
    }

    [Fact]
    public async Task GetAllTodosAsync_ShouldReturnListOfTodos()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        var todos = new List<Todo>
        {
            new Todo { Id = 1, Title = "Todo 1", Description = "Desc 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new Todo { Id = 2, Title = "Todo 2", Description = "Desc 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };

        mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(todos);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.GetAllTodosAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetTodoByIdAsync_WhenExists_ShouldReturnTodo()
    {
        // Arrange
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

        // Act
        var result = await service.GetTodoByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Title);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Todo?)null);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.GetTodoByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenExists_ShouldReturnUpdatedTodo()
    {
        // Arrange
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
            IsCompleted = updateDto.IsCompleted
        };

        mockRepository.Setup(r => r.UpdateAsync(1, It.IsAny<Todo>()))
            .ReturnsAsync(updatedTodo);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.UpdateTodoAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.True(result.IsCompleted);
    }

    [Fact]
    public async Task UpdateTodoAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
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

        // Act
        var result = await service.UpdateTodoAsync(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTodoAsync_WhenExists_ShouldReturnTrue()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.DeleteTodoAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTodoAsync_WhenNotExists_ShouldReturnFalse()
    {
        // Arrange
        var mockRepository = new Mock<ITodoRepository>();
        var mockLogger = new Mock<ILogger<TodoService>>();
        
        mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var service = new TodoService(mockRepository.Object, mockLogger.Object);

        // Act
        var result = await service.DeleteTodoAsync(999);

        // Assert
        Assert.False(result);
    }
}
