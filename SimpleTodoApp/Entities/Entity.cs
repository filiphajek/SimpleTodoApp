using Azure;
using Azure.Data.Tables;

namespace SimpleTodoApp.Entities;

public interface IEntity : ITableEntity
{
    int Id { get; }
}

public class Entity : IEntity
{
    public int Id { get; set; }
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
