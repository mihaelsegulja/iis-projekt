using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IISNotionSearch.Application.DTOs.Notion;

public class NotionObjectResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("created_time")]
    public DateTimeOffset CreatedTime { get; set; }

    [JsonPropertyName("last_edited_time")]
    public DateTimeOffset LastEditedTime { get; set; }

    [JsonPropertyName("in_trash")]
    public bool InTrash { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public JsonElement? Icon { get; set; }

    [JsonPropertyName("cover")]
    public JsonElement? Cover { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, JsonElement>? Properties { get; set; }
}
