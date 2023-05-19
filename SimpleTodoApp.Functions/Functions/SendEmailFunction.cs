using Microsoft.Azure.WebJobs;
using SimpleTodoApp.Functions.Models;
using SimpleTodoApp.Functions.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleTodoApp.Functions.Functions;

public class SendEmailFunction
{
    private readonly EmailService emailService;

    public SendEmailFunction(EmailService emailService)
    {
        this.emailService = emailService;
    }

    [FunctionName("SendEmail")]
    public async Task Run([ServiceBusTrigger("notifications", Connection = "AsbConnectionString")] string myQueueItem, int deliveryCount, DateTime enqueuedTimeUtc, string messageId)
    {
        var message = JsonSerializer.Deserialize<TodoItemUserNotification>(myQueueItem);
        await emailService.SendEmail(message.Email, message.FullName, message.Message);
    }
}
