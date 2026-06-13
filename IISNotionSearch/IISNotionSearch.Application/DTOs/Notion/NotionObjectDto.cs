namespace IISNotionSearch.Application.DTOs.Notion;

public class NotionObjectDto
{
    public string NotionId { get; set; } = null!;
    public string ObjectType { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Cover { get; set; }
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastEditedTime { get; set; } = DateTimeOffset.UtcNow;
    public bool InTrash { get; set; }
}