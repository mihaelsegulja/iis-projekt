using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Domain.Entities;

namespace IISNotionSearch.Application.Mappers;

public static class DomainMappingExtensions
{
    public static NotionObjectDto ToDto(this NotionObject entity)
    {
        return new NotionObjectDto
        {
            NotionId = entity.NotionId,
            ObjectType = entity.ObjectType,
            Title = entity.Title,
            Url = entity.Url,
            Icon = entity.Icon,
            Cover = entity.Cover,
            CreatedTime = entity.CreatedTime,
            LastEditedTime = entity.LastEditedTime,
            InTrash = entity.InTrash
        };
    }
}

