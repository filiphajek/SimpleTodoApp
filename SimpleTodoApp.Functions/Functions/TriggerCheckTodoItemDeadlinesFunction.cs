using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SimpleTodoApp.Functions.Services;
using System.Threading.Tasks;

namespace SimpleTodoApp.Functions.Functions;

public class TriggerCheckTodoItemDeadlinesFunction
{
    private readonly DeadlineNotificationService notificationService;

    public TriggerCheckTodoItemDeadlinesFunction(DeadlineNotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    [FunctionName("TriggerCheckTodoItemDeadlines")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
    {
        await notificationService.SendNotificationsToServiseBus();
        return new OkResult();
    }
}
