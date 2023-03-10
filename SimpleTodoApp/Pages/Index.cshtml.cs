using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.Entities;
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
        public ICollection<TodoItem> Todos { get; private set; } = Array.Empty<TodoItem>();

        public async Task OnGetAsync()
        {
            var userId = GetUserId();
            Todos = await dbContext.Todos.Where(i => i.UserId == userId).ToListAsync();
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
            await dbContext.Todos.Where(i => i.Id == id).ExecuteDeleteAsync();
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