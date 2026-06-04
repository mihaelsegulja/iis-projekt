using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using IISNotionSearch.Application.DTOs.Notion;

namespace IISNotionSearch.Infrastructure.Mappers;

public static class NotionMappingExtensions
{
    public static NotionObjectDto ToDto(this NotionObjectResponseDto response)
    {
        var title = ExtractTitleFromProperties(response.Properties);

        return new NotionObjectDto
        {
            NotionId = response.Id,
            ObjectType = response.Object,
            Title = title ?? "Untitled",
            Url = response.Url,
            Icon = ExtractIconString(response.Icon),
            Cover = ExtractCoverString(response.Cover),
            CreatedTime = response.CreatedTime,
            LastEditedTime = response.LastEditedTime,
            InTrash = response.InTrash
        };
    }

    public static IEnumerable<NotionObjectDto> ToDtos(this NotionSearchResponseDto response)
    {
        if (response.Results == null)
        {
            return Enumerable.Empty<NotionObjectDto>();
        }

        return response.Results.Select(r => r.ToDto());
    }

    #region Private methods

    private static string? ExtractTitleFromProperties(Dictionary<string, JsonElement>? properties)
    {
        if (properties == null)
        {
            return null;
        }

        foreach (var prop in properties.Values)
        {
            if (!prop.TryGetProperty("type", out var typeProp) || typeProp.GetString() != "title")
            {
                continue;
            }

            if (!prop.TryGetProperty("title", out var titleArray) || titleArray.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var element in titleArray.EnumerateArray())
            {
                if (element.TryGetProperty("plain_text", out var plainText))
                {
                    return plainText.GetString();
                }
            }
        }

        return null;
    }

    private static string? ExtractIconString(JsonElement? icon)
    {
        if (icon == null) return null;

        var el = icon.Value;
        if (!el.TryGetProperty("type", out var typeProp)) return null;

        var type = typeProp.GetString();
        if (type == "emoji" && el.TryGetProperty("emoji", out var emoji))
            return emoji.GetString();

        if ((type == "external" || type == "file") &&
            el.TryGetProperty(type, out var file) &&
            file.TryGetProperty("url", out var url))
            return url.GetString();

        return null;
    }

    private static string? ExtractCoverString(JsonElement? cover)
    {
        if (cover == null) return null;

        var el = cover.Value;
        if (!el.TryGetProperty("type", out var typeProp)) return null;

        var type = typeProp.GetString();
        if ((type == "external" || type == "file") &&
            el.TryGetProperty(type, out var file) &&
            file.TryGetProperty("url", out var url))
            return url.GetString();

        return null;
    }

    #endregion
}
