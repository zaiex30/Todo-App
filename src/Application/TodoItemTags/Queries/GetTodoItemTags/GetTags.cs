using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Todo_App.Application.Common.Interfaces;

namespace Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
public class GetTags
{
    public async Task<TodoItemTagVm> GetAllTags(IApplicationDbContext _context, IMapper _mapper, CancellationToken cancellationToken)
    {
        var tagList = new TodoItemTagVm
        {
            TodoItemTagList = await _context.TodoItemTags
                    .OrderBy(x => x.TagId)
                    .ProjectTo<TodoItemTagsDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken),

            TagList = await _context.TodoTags
                    .OrderBy(x => x.Id)
                    .ProjectTo<TodoTagsDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken)
        };

        return tagList;
    }
}
