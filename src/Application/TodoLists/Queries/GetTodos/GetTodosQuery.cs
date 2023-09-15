using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Todo_App.Application.Common.Extensions;
using Todo_App.Application.Common.Interfaces;
using Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.TodoLists.Queries.GetTodos;

public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly GetTags _getTags;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _getTags = new GetTags();
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var todosvm = new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = await _context.TodoLists
                .AsNoTracking()
                .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };

        var todoItemTagVm = await _getTags.GetAllTags(_context, _mapper, cancellationToken);

        var tagList =
            from tags in todoItemTagVm.TodoItemTagList
            join itemTags in todoItemTagVm.TagList on tags.TagId equals itemTags.Id into joinedTags
            from tag in joinedTags.DefaultIfEmpty()
            select new TodoItemTagVm
            {
                TodoItemTag = tags,
                TagName = tag != null ? tag.TagName : string.Empty
            };

        foreach (var item in todosvm.Lists)
        {
            item.Items = item.Items.AssignTags(tagList);
        }

        return todosvm;
    }
}
