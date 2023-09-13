using Microsoft.EntityFrameworkCore;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.Common.Extensions;
public static class TodoListExtensions
{
    public static async Task<List<TodoListDto>> FilterToDoItemsByDeletionStatus(this IQueryable<TodoListDto> todoListDtos, CancellationToken cancellationToken, string status)
    {
        var list = await todoListDtos.ToListAsync(cancellationToken);
        foreach (var item in list)
        {
            item.Items = item.Items.Where(x => x.ForDeletion == status).ToList();
        }

        return list;
    }
}
