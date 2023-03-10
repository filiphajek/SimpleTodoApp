namespace SimpleTodoApp.Entities;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
