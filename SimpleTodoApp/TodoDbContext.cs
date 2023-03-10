using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.Entities;

namespace SimpleTodoApp;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TodoItem> Todos => Set<TodoItem>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        base.OnModelCreating(modelBuilder);
    }
}