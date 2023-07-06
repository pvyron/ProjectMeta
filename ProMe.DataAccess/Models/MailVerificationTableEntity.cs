using Azure;
using Azure.Data.Tables;

namespace ProMe.DataAccess.Models;
public class MailVerificationTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = null!;
    public string RowKey { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public required DateTimeOffset Expiration { get; set; }
}
