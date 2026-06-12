using System.Collections.Generic;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Models;
using IISNotionSearch.Infrastructure.Mappers;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.Infrastructure.Services;

public class ExternalNotionService : INotionService
{
    private readonly NotionHttpClient _httpClient;
    private readonly NotionConfig _config;

    public ExternalNotionService(NotionHttpClient httpClient, IOptions<NotionConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
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
            var apiRequest = BuildCreateRequest(request, _config.ParentPageId);
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
            var apiRequest = BuildUpdateRequest(request);
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

    #region Private methods

    private static object BuildCreateRequest(CreateNotionPageDto dto, string parentPageId)
    {
        var title = string.IsNullOrWhiteSpace(dto.Title) ? "New Page" : dto.Title;

        var request = new Dictionary<string, object?>
        {
            ["parent"] = new { type = "page_id", page_id = parentPageId },
            ["properties"] = new Dictionary<string, object>
            {
                ["title"] = new
                {
                    title = new[]
                    {
                        new { type = "text", text = new { content = title } }
                    }
                }
            }
        };

        if (!string.IsNullOrWhiteSpace(dto.Icon))
            request["icon"] = new { type = "emoji", emoji = dto.Icon };

        if (!string.IsNullOrWhiteSpace(dto.Cover))
            request["cover"] = new { type = "external", external = new { url = dto.Cover } };

        return request;
    }

    private static object BuildUpdateRequest(UpdateNotionPageDto dto)
    {
        var body = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            body["properties"] = new Dictionary<string, object>
            {
                ["title"] = new
                {
                    title = new[]
                    {
                        new { type = "text", text = new { content = dto.Title } }
                    }
                }
            };
        }

        if (!string.IsNullOrWhiteSpace(dto.Icon))
            body["icon"] = new { type = "emoji", emoji = dto.Icon };

        if (!string.IsNullOrWhiteSpace(dto.Cover))
            body["cover"] = new { type = "external", external = new { url = dto.Cover } };

        return body;
    }

    #endregion
}