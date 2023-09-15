using Todo_App.Application.Common.Mappings;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
public class TodoItemTagsDto : IMapFrom<TodoItemTag>
{
    public int Id { get; set; }
    public int TagId { get; set; }
    public int TodoItemTagId {  get; set; }
}
