using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleTodoApp.Entities;
using System.Security.Claims;

namespace SimpleTodoApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TableClient tableClient;

        public IndexModel(TableClient tableClient)
        {
            this.tableClient = tableClient;
        }

        [BindProperty]
        public ICollection<TodoItem> Todos { get; private set; } = Array.Empty<TodoItem>();

        public async Task OnGetAsync()
        {
            Todos = await GetItems();
        }

        public async Task<IActionResult> OnPostAsync(TodoItem todo)
        {
            if (string.IsNullOrEmpty(todo.Description))
            {
                ModelState.Clear();
                ModelState.AddModelError(nameof(todo.Description), "Description is required");
                return RedirectToPage();
            }
            var nameClaimValue = User.FindFirstValue(ClaimTypes.Name)!;
            todo.PartitionKey = nameClaimValue;

            var todos = await GetItems();
            todo.Id = todos.Any() ? todos.Max(i => i.Id) + 1 : 1;
            todo.RowKey = GetRowKey(todo.Id);

            await tableClient.AddEntityAsync(todo);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var nameClaimValue = User.FindFirstValue(ClaimTypes.Name)!;
            await tableClient.DeleteEntityAsync(nameClaimValue, GetRowKey(id));
            return RedirectToPage();
        }

        public RedirectToPageResult OnPostEditAsync(int id)
        {
            return RedirectToPage("Edit", new { id = id });
        }

        private async Task<List<TodoItem>> GetItems()
        {
            var result = new List<TodoItem>();
            var nameClaimValue = User.FindFirstValue(ClaimTypes.Name)!;
            var pages = tableClient.QueryAsync<TableEntity>(i => i.PartitionKey == nameClaimValue, 20);

            await foreach (var page in pages.AsPages())
            {
                var tmp = page.Values.Where(i => !i.ContainsKey(nameof(Entities.User.Name))).Select(i => new TodoItem
                {
                    Description = i[nameof(TodoItem.Description)].ToString()!,
                    Id = (int)i[nameof(TodoItem.Id)]!,
                    RowKey = i[nameof(TableEntity.RowKey)].ToString()!,
                    PartitionKey = i[nameof(TableEntity.PartitionKey)].ToString()!,
                });
                result.AddRange(tmp);
            }
            return result;
        }

        private string GetRowKey(int id)
        {
            var idStr = id.ToString();
            var prefix = string.Join("", Enumerable.Repeat("0", 4 - idStr.Length));
            return prefix + id;
        }
    }
}