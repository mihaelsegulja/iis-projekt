using IISNotionSearch.Domain.Abstractions;

namespace IISNotionSearch.Domain.Entities;

public class NotionObject : BaseEntity
{
    public string NotionId { get; set; } = null!;
    public string ObjectType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Cover { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastEditedTime { get; set; }
    
    public string? RawJson { get; set; }
}