using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.DAL;
using System.Security.Claims;

namespace SimpleTodoApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TodoDbContext dbContext;

        public IndexModel(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [BindProperty]
        public User UserModel { get; set; } = new();

        [BindProperty]
        public ICollection<TodoItem> Todos { get; private set; } = Array.Empty<TodoItem>();

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            Todos = await dbContext.Todos.Where(i => i.UserId == userId).ToListAsync();
            UserModel = await dbContext.Users.FirstAsync(i => i.Id == userId);
        }

        public async Task<IActionResult> OnPostEditMailAsync(User user)
        {
            var userId = GetUserId();
            var userEntity = await dbContext.Users.FirstAsync(i => i.Id == userId);
            userEntity.Email = user.Email;
            dbContext.Update(userEntity);
            await dbContext.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(TodoItem todo)
        {
            if (string.IsNullOrEmpty(todo.Description))
            {
                ModelState.Clear();
                ModelState.AddModelError(nameof(todo.Description), "Description is required");
                return RedirectToPage();
            }
            todo.UserId = GetUserId();
            await dbContext.Todos.AddAsync(todo);
            await dbContext.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var toRemove = await dbContext.Todos.Where(i => i.Id == id).ToListAsync();
            foreach (var item in toRemove)
            {
                dbContext.Remove(item);
            }
            await dbContext.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            if (!await dbContext.Todos.AnyAsync(i => i.Id == id))
            {
                return RedirectToPage();
            }
            return RedirectToPage("Edit", new { id = id });
        }

        private int GetUserId() => int.Parse(User.FindFirstValue("userId")!);
    }
}