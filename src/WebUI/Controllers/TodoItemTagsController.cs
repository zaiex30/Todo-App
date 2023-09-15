using Microsoft.AspNetCore.Mvc;
using Todo_App.Application.TodoItemTags.Commands.AddTodoItemTag;
using Todo_App.Application.TodoItemTags.Commands.DeleteTodoItemTag;
using Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;

namespace Todo_App.WebUI.Controllers;

public class TodoItemTagsController : ApiControllerBase

{
    [HttpGet]
    public async Task<ActionResult<TodoItemTagVm>> Get()
    {
        return await Mediator.Send(new GetTodoItemTagsQuery());
    }

    [HttpPost]
    public async Task<int> AddItemTag(AddTodoItemTagCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTodoItemTagCommand(id));

        return NoContent();
    }
}
