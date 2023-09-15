namespace Todo_App.Application.TodoItemTags.Queries.GetTodoItemTags;
public class TodoItemTagVm
{
    public List<TodoTagsDto> TagList { get; set; } = new List<TodoTagsDto>();
    public List<TodoItemTagsDto> TodoItemTagList { get; set; } = new List<TodoItemTagsDto>();

    public TodoItemTagsDto TodoItemTag { get; set; } = new TodoItemTagsDto();
    public string? TagName { get; set; }
}
