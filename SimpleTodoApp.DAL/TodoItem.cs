namespace SimpleTodoApp.DAL;

public class TodoItem : Entity
{
    public string Description { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime Deadline { get; set; }
    public User User { get; set; } = default!;
}
