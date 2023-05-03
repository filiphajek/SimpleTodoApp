using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleTodoApp.Entities;
using System.Security.Claims;

namespace SimpleTodoApp.Pages
{
    public class EditModel : PageModel
    {
        private readonly TableClient tableClient;

        public EditModel(TableClient tableClient)
        {
            this.tableClient = tableClient;
        }

        public TodoItem EditTodo { get; private set; } = default!;

        public async Task OnGetAsync(int id)
        {
            var nameClaimValue = User.FindFirstValue(ClaimTypes.Name)!;
            var idStr = id.ToString();
            var prefix = string.Join("", Enumerable.Repeat("0", 4 - idStr.Length));
            EditTodo = await tableClient.GetEntityAsync<TodoItem>(nameClaimValue, prefix + id);
        }

        public async Task<IActionResult> OnPostAsync(TodoItem todo)
        {
            if (string.IsNullOrEmpty(todo.Description))
            {
                ModelState.Clear();
                ModelState.AddModelError(nameof(todo.Description), "Description is required");
                return RedirectToPage(new { id = todo.Id });
            }

            todo.PartitionKey = User.FindFirstValue(ClaimTypes.Name)!;
            var idStr = todo.Id.ToString();
            var prefix = string.Join("", Enumerable.Repeat("0", 4 - idStr.Length));
            todo.RowKey = prefix + idStr;

            await tableClient.UpsertEntityAsync(todo);
            return RedirectToPage("Index");
        }
    }
}