using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
[EnableCors("AllowReactApp")]
public class TodosController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public TodosController()
    {
        _dbContext = new DatabaseContext("Data Source=todos.db");
    }

    [HttpGet]
    public ActionResult<IEnumerable<Todo>> Get()
    {
        return _dbContext.GetTodos();
    }

    [HttpPost]
    public IActionResult Post([FromBody] Todo todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Task))
        {
            return BadRequest("Task field is required.");
        }

        if (string.IsNullOrWhiteSpace(todo.Descriptions))
        {
            return BadRequest("Descriptions field is required.");
        }

        _dbContext.AddTodo(todo.Task, todo.Descriptions);
        return Ok();
    }

    [HttpPut("{Id}/update-task")]
    public IActionResult UpdateTodo(int id, [FromBody] Todo todo)
    {
        _dbContext.UpdateTodo(id, todo.Task, todo.Descriptions);
        return Ok();
    }

    [HttpPut("{Id}/update-status")]
    public IActionResult UpdateTodoStatus(int Id, [FromBody] Todo todo)
    {
        Console.WriteLine($"Updating todo {Id} to {todo.IsCompleted}");
        _dbContext.UpdateTodoStatus(Id,todo.Task,todo.Descriptions, todo.IsCompleted);
        return Ok();
    }

    [HttpDelete("{Id}")]
    public IActionResult Delete(int Id)
    {
        _dbContext.DeleteTodo(Id);
        return Ok();
    }
}