using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Models;
using IISNotionSearch.Infrastructure.Mappers;

namespace IISNotionSearch.Infrastructure.Services;

public class ExternalNotionService : INotionService
{
    private readonly NotionHttpClient _httpClient;

    public ExternalNotionService(NotionHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StandardResponse<IEnumerable<NotionObjectDto>>> SearchAsync(string? query = null)
    {
        try
        {
            var response = await _httpClient.SearchAsync(query);
            var dtos = response?.ToDtos() ?? Enumerable.Empty<NotionObjectDto>();
            return StandardResponse<IEnumerable<NotionObjectDto>>.Create(ResultStatus.Ok, dtos);
        }
        catch (Exception ex)
        {
            return StandardResponse<IEnumerable<NotionObjectDto>>.Create(ResultStatus.InternalError, message: ex.Message);
        }
    }

    public async Task<StandardResponse<NotionObjectDto>> GetPageAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetPageAsync(id);
            var dto = response?.ToDto();
            if (dto == null)
            {
                return StandardResponse<NotionObjectDto>.Create(ResultStatus.NotFound, message: "Page not found");
            }
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.Ok, dto);
        }
        catch (Exception ex)
        {
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.InternalError, message: ex.Message);
        }
    }

    public async Task<StandardResponse<NotionObjectDto>> CreatePageAsync(object pageData)
    {
        try
        {
            var title = pageData is NotionObjectDto dto && !string.IsNullOrWhiteSpace(dto.Title)
                ? dto.Title
                : "New Page";

            var request = BuildTitleProperties(title);
            var response = await _httpClient.CreatePageAsync(request);
            var resultDto = response?.ToDto() ?? new NotionObjectDto
            {
                NotionId = string.Empty,
                ObjectType = "page",
                Title = title,
                Url = string.Empty,
                CreatedTime = DateTimeOffset.UtcNow,
                LastEditedTime = DateTimeOffset.UtcNow,
                InTrash = false
            };
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.Created, resultDto);
        }
        catch (Exception ex)
        {
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.InternalError, message: ex.Message);
        }
    }

    public async Task<StandardResponse<NotionObjectDto>> UpdatePageAsync(string id, object pageData)
    {
        try
        {
            var title = pageData is NotionObjectDto dto && !string.IsNullOrWhiteSpace(dto.Title)
                ? dto.Title
                : "Untitled";

            var request = BuildTitleProperties(title);
            var response = await _httpClient.UpdatePageAsync(id, request);
            var resultDto = response?.ToDto() ?? new NotionObjectDto
            {
                NotionId = id,
                ObjectType = "page",
                Title = title,
                Url = string.Empty,
                CreatedTime = DateTimeOffset.UtcNow,
                LastEditedTime = DateTimeOffset.UtcNow,
                InTrash = false
            };
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.Ok, resultDto);
        }
        catch (Exception ex)
        {
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.InternalError, message: ex.Message);
        }
    }

    public async Task<StandardResponse<bool>> DeletePageAsync(string id)
    {
        try
        {
            await _httpClient.TrashPageAsync(id);
            return StandardResponse<bool>.Create(ResultStatus.Ok, true);
        }
        catch (Exception ex)
        {
            return StandardResponse<bool>.Create(ResultStatus.InternalError, false, message: ex.Message);
        }
    }

    private static object BuildTitleProperties(string title)
    {
        return new
        {
            properties = new
            {
                Title = new
                {
                    title = new[]
                    {
                        new
                        {
                            text = new
                            {
                                content = title
                            }
                        }
                    }
                }
            }
        };
    }
}