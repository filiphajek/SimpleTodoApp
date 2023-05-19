using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.DAL;

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

builder.Services.AddDbContext<TodoDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
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

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

if (await dbContext.Database.EnsureCreatedAsync())
{
    var user = await dbContext.AddAsync(new User { Name = "Tom", Email = "filiphajek268@gmail.com", Password = BCrypt.Net.BCrypt.HashPassword("123") });
    await dbContext.SaveChangesAsync();

    await dbContext.AddAsync(new TodoItem { Description = "First item", UserId = user.Entity.Id, Deadline = DateTime.UtcNow.AddHours(10) });
    await dbContext.SaveChangesAsync();
}
app.Run();
