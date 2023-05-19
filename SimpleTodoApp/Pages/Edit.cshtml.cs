using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SimpleTodoApp.DAL;
using System.Security.Claims;

namespace SimpleTodoApp.Pages
{
    public class EditModel : PageModel
    {
        private readonly TodoDbContext dbContext;

        public EditModel(TodoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TodoItem EditTodo { get; private set; } = default!;

        public async Task OnGetAsync(int id)
        {
            EditTodo = await dbContext.Todos.FirstAsync(i => i.Id == id);
        }

        public async Task<IActionResult> OnPostAsync(TodoItem todo)
        {
            todo.UserId = int.Parse(User.FindFirstValue("userId")!);
            if (string.IsNullOrEmpty(todo.Description))
            {
                ModelState.Clear();
                ModelState.AddModelError(nameof(todo.Description), "Description is required");
                return RedirectToPage(new { id = todo.Id });
            }

            dbContext.Update(todo);
            await dbContext.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}