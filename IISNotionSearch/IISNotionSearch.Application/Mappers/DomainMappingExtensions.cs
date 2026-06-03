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
    
    public static NotionObject ToEntity(this NotionObjectDto dto)
    {
        return new NotionObject
        {
            NotionId = dto.NotionId,
            ObjectType = dto.ObjectType,
            Title = dto.Title,
            Url = dto.Url,
            Icon = dto.Icon,
            Cover = dto.Cover,
            CreatedTime = dto.CreatedTime,
            LastEditedTime = dto.LastEditedTime,
            InTrash = dto.InTrash
        };
    }
}

