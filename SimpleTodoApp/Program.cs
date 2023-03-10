using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp;

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
    c.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
