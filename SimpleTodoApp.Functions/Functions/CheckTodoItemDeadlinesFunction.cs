using Microsoft.Azure.WebJobs;
using SimpleTodoApp.Functions.Services;
using System.Threading.Tasks;

namespace SimpleTodoApp.Functions.Functions;

public class CheckTodoItemDeadlinesFunction
{
    private readonly DeadlineNotificationService notificationService;

    public CheckTodoItemDeadlinesFunction(DeadlineNotificationService notificationService)
    {
        this.notificationService = notificationService;
    }

    [FunctionName("CheckTodoItemDeadlines")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        await notificationService.SendNotificationsToServiseBus();
    }
}
