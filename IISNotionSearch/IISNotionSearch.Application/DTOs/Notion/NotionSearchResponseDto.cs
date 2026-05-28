using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IISNotionSearch.Application.DTOs.Notion;

public class NotionSearchResponseDto
{
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("results")]
    public List<NotionObjectResponseDto> Results { get; set; } = new();

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
