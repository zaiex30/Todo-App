using Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
using Todo_App.Application.TodoLists.Queries.GetTodos;

namespace Todo_App.Application.Common.Extensions;
public static class TodoItemExtensions
{
    public static List<TodoItemDto> AssignTags(this IList<TodoItemDto> todoItemDtos, IEnumerable<TodoItemTagVm> todoItemTagVms)
    {
        var items = todoItemDtos.ToList();

        foreach(var item in items)
        {
            item.Tags = todoItemTagVms.Where(x => x.TodoItemTag.TodoItemTagId == item.Id).ToList() ?? new List<TodoItemTagVm>();
        }

        return items;
    }
}
