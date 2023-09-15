using MediatR;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItemTags.Commands.AddTodoItemTag;

public record AddTodoItemTagCommand : IRequest<int>
{
    public int TagId { get; init; }

    public int TodoItemTagId { get; init; }
}

public class AddTodoItemTagCommandHandler : IRequestHandler<AddTodoItemTagCommand, int>
{
    private readonly IApplicationDbContext _context;

    public AddTodoItemTagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(AddTodoItemTagCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItemTag
        {
            TagId = request.TagId,
            TodoItemTagId = request.TodoItemTagId
        };

        _context.TodoItemTags.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
