using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todos.api.Data;
using todos.api.Models;
using Microsoft.Extensions.Logging;
using todos.api.Common;
using Microsoft.Extensions.Options;

namespace todos.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TodosController> _logger;
        private readonly AppSettings _appSettings;
        public TodosController(AppDbContext context, ILogger<TodosController> logger, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            //var source = _appSettings.FileDirectory.Source;
            var todos = await _context.Todos.ToListAsync();
            return Ok(todos);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodo([FromBody] Todo newTodo)
        {
            var source = _appSettings.FileDirectory.Source;

            var guid = Guid.NewGuid().ToString();

            var filePath = Path.Combine(source, $"{guid}.txt");

            var todoContent = $"Title: {newTodo.Title}";

            // Ensure the directory exists
            if (!Directory.Exists(source))
            {
                Directory.CreateDirectory(source);
            }

            // Write contents to the file
            await System.IO.File.WriteAllTextAsync(filePath, todoContent);

            _logger.LogInformation("created file for new todo...");

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
