using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todos.api.Data;
using todos.api.Models;
using Microsoft.Extensions.Logging;

namespace todos.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TodosController> _logger;
        public TodosController(AppDbContext context, ILogger<TodosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var todos = await _context.Todos.ToListAsync();
            return Ok(todos);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodo([FromBody] Todo newTodo)
        {
            // Add the new Todo to the database
            _context.Todos.Add(newTodo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added new todo...");

            return CreatedAtAction(nameof(GetTodos), new { id = newTodo.Id}, newTodo);  
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo updateTodo)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if(todo == null)
            {
                return NotFound();
            }

            todo.Title = updateTodo.Title;
            todo.IsComplete = updateTodo.IsComplete;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated existing todo...");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);

            if(todo == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted existing todo...");
            return NoContent();
        }
    }
}
