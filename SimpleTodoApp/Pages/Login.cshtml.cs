using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.DAL;
using System.Security.Claims;

namespace SimpleTodoApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly TodoDbContext dbContext;

        public LoginModel(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> OnPostAsync(User user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            var existingUser = await dbContext.Users.FirstOrDefaultAsync(i => i.Name == user.Name);
            if (existingUser is not null)
            {
                if (BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
                {
                    await SignInAsync(existingUser.Id, existingUser.Name);
                    return RedirectToPage("/Index");
                }
                ModelState.AddModelError(nameof(user.Name), "Wrong password");
                return Page();
            }

            var userEntity = await dbContext.Users.AddAsync(new User
            {
                Name = user.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });
            await dbContext.SaveChangesAsync();
            await SignInAsync(userEntity.Entity.Id, userEntity.Entity.Name);
            return RedirectToPage("/Index");
        }

        private async Task SignInAsync(int id, string name)
        {
            await HttpContext.SignInAsync("default", new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim("userId", id.ToString()),
                    new Claim(ClaimTypes.Name, name),
                }, "default")),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                });
        }
    }
}