using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IISNotionSearch.Application.DTOs.Notion;

namespace IISNotionSearch.Infrastructure.Services;

public class NotionHttpClient
{
    private readonly HttpClient _httpClient;

    public NotionHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<NotionSearchResponseDto?> SearchAsync(string? query = null, CancellationToken cancellationToken = default)
    {
        object request = string.IsNullOrWhiteSpace(query) ? new { } : new { query };
        var response = await _httpClient.PostAsJsonAsync("search", request, cancellationToken);
        await EnsureSuccessWithBodyAsync(response);
        return await response.Content.ReadFromJsonAsync<NotionSearchResponseDto>(cancellationToken: cancellationToken);
    }

    public async Task<NotionObjectResponseDto?> GetPageAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"pages/{pageId}", cancellationToken);
        await EnsureSuccessWithBodyAsync(response);
        return await response.Content.ReadFromJsonAsync<NotionObjectResponseDto>(cancellationToken: cancellationToken);
    }

    public async Task<NotionObjectResponseDto?> CreatePageAsync(object pageData, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("pages", pageData, cancellationToken);
        await EnsureSuccessWithBodyAsync(response);
        return await response.Content.ReadFromJsonAsync<NotionObjectResponseDto>(cancellationToken: cancellationToken);
    }

    public async Task<NotionObjectResponseDto?> UpdatePageAsync(string pageId, object pageData, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PatchAsJsonAsync($"pages/{pageId}", pageData, cancellationToken);
        await EnsureSuccessWithBodyAsync(response);
        return await response.Content.ReadFromJsonAsync<NotionObjectResponseDto>(cancellationToken: cancellationToken);
    }

    public async Task TrashPageAsync(string pageId, CancellationToken cancellationToken = default)
    {
        var request = new { in_trash = true, archived = true };
        var response = await _httpClient.PatchAsJsonAsync($"pages/{pageId}", request, cancellationToken);
        await EnsureSuccessWithBodyAsync(response);
    }

    #region Private methods
    
    private static async Task EnsureSuccessWithBodyAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Notion API returned {response.StatusCode}: {body}");
        }
    }

    #endregion
}
