using System.Collections.Generic;
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

    public async Task<StandardResponse<NotionObjectDto>> CreatePageAsync(CreateNotionPageDto request)
    {
        try
        {
            var title = string.IsNullOrWhiteSpace(request.Title) ? "New Page" : request.Title;
            var apiRequest = BuildCreateRequest(title, request.Icon, request.Cover);
            var response = await _httpClient.CreatePageAsync(apiRequest);
            var dto = response?.ToDto();

            if (dto == null)
            {
                return StandardResponse<NotionObjectDto>.Create(ResultStatus.InternalError,
                    message: "Failed to parse Notion API response");
            }

            return StandardResponse<NotionObjectDto>.Create(ResultStatus.Created, dto);
        }
        catch (Exception ex)
        {
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.InternalError, message: ex.Message);
        }
    }

    public async Task<StandardResponse<NotionObjectDto>> UpdatePageAsync(string id, UpdateNotionPageDto request)
    {
        try
        {
            var apiRequest = BuildUpdateRequest(request.Title);
            var response = await _httpClient.UpdatePageAsync(id, apiRequest);
            var dto = response?.ToDto();

            if (dto == null)
            {
                return StandardResponse<NotionObjectDto>.Create(ResultStatus.InternalError,
                    message: "Failed to parse Notion API response");
            }

            return StandardResponse<NotionObjectDto>.Create(ResultStatus.Ok, dto);
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

    private static object BuildCreateRequest(string title, string? icon, string? cover)
    {
        var request = new Dictionary<string, object?>
        {
            ["parent"] = new { type = "page_id", page_id = "" },
            ["properties"] = new Dictionary<string, object>
            {
                ["Title"] = new
                {
                    title = new[]
                    {
                        new { text = new { content = title } }
                    }
                }
            }
        };

        if (!string.IsNullOrWhiteSpace(icon))
            request["icon"] = new { emoji = icon };

        if (!string.IsNullOrWhiteSpace(cover))
            request["cover"] = new { external = new { url = cover } };

        return request;
    }

    private static object BuildUpdateRequest(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new { };

        return new
        {
            properties = new
            {
                Title = new
                {
                    title = new[]
                    {
                        new { text = new { content = title } }
                    }
                }
            }
        };
    }
}