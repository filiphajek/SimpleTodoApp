using Azure.Data.Tables;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleTodoApp.Entities;
using System.Security.Claims;

namespace SimpleTodoApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly TableClient tableClient;

        public LoginModel(TableClient tableClient)
        {
            this.tableClient = tableClient;
        }

        public async Task<IActionResult> OnPostAsync(User user)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            var existingUser = await tableClient.GetEntityIfExistsAsync<User>(user.Name, string.Empty);
            if (existingUser.HasValue)
            {
                if (BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Value.Password))
                {
                    await SignInAsync(existingUser.Value.Name);
                    return RedirectToPage("/Index");
                }
                ModelState.AddModelError(nameof(user.Name), "Wrong password");
                return Page();
            }

            var userEntity = new User
            {
                Name = user.Name,
                PartitionKey = user.Name,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            };
            await tableClient.AddEntityAsync(userEntity);
            await SignInAsync(userEntity.Name);
            return RedirectToPage("/Index");
        }

        private async Task SignInAsync(string name)
        {
            await HttpContext.SignInAsync("default", new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, name),
                }, "default")),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                });
        }
    }
}