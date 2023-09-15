namespace Todo_App.Domain.Entities;
public class TodoItemTag : BaseAuditableEntity
{
    public int TagId { get; set; }
    public int TodoItemTagId { get; set; }
}
