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
            Icon = null,
            Cover = null,
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
}
