using Azure;
using Azure.Data.Tables;

namespace ProMe.DataAccess.Models;
public class RefreshTokenTableEntity : ITableEntity
{
    public required string PartitionKey { get; set; } = null!;
    public required string RowKey { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public DateTimeOffset ValidFrom { get; set; } = DateTimeOffset.UtcNow;
    public required DateTimeOffset ValidUntil { get; set; }
}
