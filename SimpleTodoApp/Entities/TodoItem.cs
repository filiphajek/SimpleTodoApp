namespace SimpleTodoApp.Entities;

public class TodoItem : Entity
{
    public string Description { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = default!;
}
