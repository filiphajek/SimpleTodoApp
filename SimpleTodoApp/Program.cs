using Azure.Data.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/");
    options.Conventions.AuthorizePage("/Index");
    options.Conventions.AllowAnonymousToPage("/Login");
});

builder.Services.AddAuthentication("default").AddCookie("default", c =>
{
    c.Cookie.Name = "todo-cookie";
    c.Cookie.SameSite = SameSiteMode.Lax;
    c.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    c.LoginPath = "/Login";
    c.LogoutPath = "/Logout";
});
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var tableServiceClient = new TableServiceClient(builder.Configuration.GetConnectionString("Default"));
var tableClient = tableServiceClient.GetTableClient(nameof(TodoItem));
await tableClient.CreateIfNotExistsAsync();
builder.Services.AddSingleton(tableClient);

//seeds:
await tableClient.UpsertEntityAsync<User>(new()
{
    Id = 1,
    Name = "Tom",
    Password = BCrypt.Net.BCrypt.HashPassword("123"),
    PartitionKey = "Tom"
});

await tableClient.UpsertEntityAsync<TodoItem>(new()
{
    Id = 1,
    Description = "Todo item 1",
    Timestamp = DateTime.Now,
    PartitionKey = "Tom",
    RowKey = "0001",
});

await tableClient.UpsertEntityAsync<TodoItem>(new()
{
    Id = 2,
    Description = "Todo item 2",
    Timestamp = DateTime.Now,
    PartitionKey = "Tom",
    RowKey = "0002",
});

await tableClient.UpsertEntityAsync<TodoItem>(new()
{
    Id = 3,
    Description = "Todo item 3",
    Timestamp = DateTime.Now,
    PartitionKey = "Tom",
    RowKey = "0003",
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
