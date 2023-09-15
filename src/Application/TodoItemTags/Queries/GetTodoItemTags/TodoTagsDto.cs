using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
public class TodoTagsDto : IMapFrom<TodoTags>
{
    public int Id { get; set; }
    public string? TagName { get; set; }
}
