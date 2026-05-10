namespace IISNotionSearch.Application.DTOs.Notion;

public class NotionSearchResponseDto
{
    public string Object { get; set; } = "list";
    public List<NotionObjectResponseDto> Results { get; set; } = new();
    public string? NextCursor { get; set; }
    public bool HasMore { get; set; }
}

public class NotionObjectResponseDto
{
    public string Object { get; set; } = null!; // "page" or "database"
    public string Id { get; set; } = null!;
    public DateTime CreatedTime { get; set; }
    public DateTime LastEditedTime { get; set; }
    public NotionIconDto? Icon { get; set; }
    public NotionCoverDto? Cover { get; set; }
    public string Url { get; set; } = null!;
    
    // Pages have properties, Databases have a title array at the top level
    public Dictionary<string, NotionPropertyDto>? Properties { get; set; }
    public List<NotionRichTextDto>? Title { get; set; } 
}

public class NotionIconDto
{
    public string Type { get; set; } = null!;
    public string? Emoji { get; set; }
    public NotionFileDto? External { get; set; }
    public NotionFileDto? File { get; set; }
}

public class NotionCoverDto
{
    public string Type { get; set; } = null!;
    public NotionFileDto? External { get; set; }
    public NotionFileDto? File { get; set; }
}

public class NotionFileDto
{
    public string Url { get; set; } = null!;
}

public class NotionPropertyDto
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    public List<NotionRichTextDto>? Title { get; set; } // Only present if Type == "title"
}

public class NotionRichTextDto
{
    public string Type { get; set; } = null!;
    public string PlainText { get; set; } = null!;
}
