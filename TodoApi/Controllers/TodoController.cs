using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    /// <summary>
    /// Controller for managing TODO items
    /// Provides RESTful API endpoints following HTTP semantics
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITodoService todoService, ILogger<TodosController> logger)
        {
            _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new TODO item
        /// </summary>
        /// <param name="createDto">The TODO item to create</param>
        /// <returns>The created TODO item</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is invalid</response>
        [HttpPost]
        [ProducesResponseType(typeof(TodoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TodoDto>> CreateTodo([FromBody] CreateTodoDto createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateTodo");
                return BadRequest(ModelState);
            }

            try
            {
                var todo = await _todoService.CreateTodoAsync(createDto);
                return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TODO item");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while creating the TODO item" });
            }
        }

        /// <summary>
        /// Retrieves all TODO items
        /// </summary>
        /// <returns>A list of all TODO items</returns>
        /// <response code="200">Returns the list of TODO items</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetAllTodos()
        {
            try
            {
                var todos = await _todoService.GetAllTodosAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving TODO items");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving TODO items" });
            }
        }

        /// <summary>
        /// Retrieves a specific TODO item by ID
        /// </summary>
        /// <param name="id">The ID of the TODO item</param>
        /// <returns>The requested TODO item</returns>
        /// <response code="200">Returns the TODO item</response>
        /// <response code="404">If the item is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoDto>> GetTodoById(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found", id);
                    return NotFound(new { message = $"TODO item with ID {id} not found" });
                }

                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving TODO item with ID {TodoId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving the TODO item" });
            }
        }

        /// <summary>
        /// Updates an existing TODO item
        /// </summary>
        /// <param name="id">The ID of the TODO item to update</param>
        /// <param name="updateDto">The updated TODO item data</param>
        /// <returns>The updated TODO item</returns>
        /// <response code="200">Returns the updated item</response>
        /// <response code="400">If the item is invalid</response>
        /// <response code="404">If the item is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoDto>> UpdateTodo(int id, [FromBody] UpdateTodoDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateTodo");
                return BadRequest(ModelState);
            }

            try
            {
                var todo = await _todoService.UpdateTodoAsync(id, updateDto);
                if (todo == null)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found for update", id);
                    return NotFound(new { message = $"TODO item with ID {id} not found" });
                }

                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TODO item with ID {TodoId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while updating the TODO item" });
            }
        }

        /// <summary>
        /// Deletes a TODO item by ID
        /// </summary>
        /// <param name="id">The ID of the TODO item to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the item was successfully deleted</response>
        /// <response code="404">If the item is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            try
            {
                var deleted = await _todoService.DeleteTodoAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("TODO item with ID {TodoId} not found for deletion", id);
                    return NotFound(new { message = $"TODO item with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting TODO item with ID {TodoId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting the TODO item" });
            }
        }
    }
}
