using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItemTags.Commands.AddTodoItemTag;

public record AddTodoItemTagCommand : IRequest<TodoItemTagVm>
{
    public int TagId { get; init; }

    public int TodoItemTagId { get; init; }
}

public class AddTodoItemTagCommandHandler : IRequestHandler<AddTodoItemTagCommand, TodoItemTagVm>
{
    private readonly IApplicationDbContext _context;

    public AddTodoItemTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoItemTagVm> Handle(AddTodoItemTagCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItemTag
        {
            TagId = request.TagId,
            TodoItemTagId = request.TodoItemTagId
        };

        _context.TodoItemTags.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        var tag = await _context.TodoTags
            .FindAsync(new object[] { entity.TagId }, cancellationToken);

        var todoItemTagEntity = new TodoItemTagVm
        {
            TagName = tag?.TagName != null ? tag.TagName : string.Empty,
            TodoItemTag = new TodoItemTagsDto
            {
                Id = entity.Id,
                TagId = entity.TagId,
                TodoItemTagId = entity.TodoItemTagId
            }
        };

        return todoItemTagEntity;
    }
}
