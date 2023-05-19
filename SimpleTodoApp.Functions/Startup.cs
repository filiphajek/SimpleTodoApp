using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleTodoApp.DAL;
using SimpleTodoApp.Functions.Services;
using System;

[assembly: FunctionsStartup(typeof(SimpleTodoApp.Functions.Startup))]

namespace SimpleTodoApp.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var connString = Environment.GetEnvironmentVariable("DbConnectionString");
        builder.Services.AddDbContext<TodoDbContext>(options =>
        {
            options.UseSqlServer(connString);
        });
        builder.Services.AddScoped<DeadlineNotificationService>();
        builder.Services.AddScoped<EmailService>();
    }
}
