using AutoMapper;
using MediatR;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;

public record GetTodoItemTagsQuery : IRequest<TodoItemTagVm>;

public class GetTodoItemTagsQueryHandler : IRequestHandler<GetTodoItemTagsQuery, TodoItemTagVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly GetTags _getTags; 

    public GetTodoItemTagsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _getTags = new GetTags();
    }

    public async Task<TodoItemTagVm> Handle(GetTodoItemTagsQuery request, CancellationToken cancellationToken)
    {
        var tagList = await _getTags.GetAllTags(_context, _mapper, cancellationToken);

        return tagList;
    }
}