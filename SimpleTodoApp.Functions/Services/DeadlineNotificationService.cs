using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.DAL;
using SimpleTodoApp.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleTodoApp.Functions.Services;

public class DeadlineNotificationService
{
    private readonly TodoDbContext dbContext;

    public DeadlineNotificationService(TodoDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task SendNotificationsToServiseBus()
    {
        var notifications = await GetTodoItemsNearDeadlients();

        var asbConnectionString = Environment.GetEnvironmentVariable("AsbConnectionString");
        await using var client = new ServiceBusClient(asbConnectionString);
        var sender = client.CreateSender("notifications");

        foreach (var notification in notifications)
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(notification));
            await sender.SendMessageAsync(message);
        }
    }

    private async Task<ICollection<TodoItemUserNotification>> GetTodoItemsNearDeadlients()
    {
        var todoItems = await dbContext.Todos
            .Include(i => i.User)
            .Where(i => i.Deadline >= DateTime.UtcNow && i.Deadline <= DateTime.UtcNow.AddDays(3))
            .ToListAsync();

        return todoItems.Select(i => new TodoItemUserNotification
        {
            FullName = i.User.Name,
            Email = i.User.Email,
            Message = $"Todo item: '{i.Description}' is close to the deadline '{i.Deadline}'"
        }).ToArray();
    }
}
