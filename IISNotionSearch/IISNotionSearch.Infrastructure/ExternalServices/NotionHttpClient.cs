using System.Net.Http.Json;
using System.Text.Json;
using IISNotionSearch.Application.Configurations;
using IISNotionSearch.Application.DTOs.Notion;
using Microsoft.Extensions.Options;

namespace IISNotionSearch.Infrastructure.ExternalServices;

public class NotionHttpClient
{
    private const string SearchEndpoint = "search";
    private const string PagesEndpoint = "pages";

    private readonly HttpClient _httpClient;
    private readonly NotionConfig _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public NotionHttpClient(HttpClient httpClient, IOptions<NotionConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;

        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.InternalIntegrationSecret}");
        _httpClient.DefaultRequestHeaders.Add("Notion-Version", _config.Version);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }
    
    public async Task<NotionSearchResponseDto?> SearchAsync(string query = "", int pageSize = 20)
    {
        var requestBody = new
        {
            query = query,
            page_size = pageSize,
            sort = new
            {
                direction = "descending",
                timestamp = "last_edited_time"
            }
        };

        var response = await _httpClient.PostAsJsonAsync(SearchEndpoint, requestBody);
        EnsureSuccess(response);

        return await response.Content.ReadFromJsonAsync<NotionSearchResponseDto>(_jsonOptions);
    }
    
    public async Task<NotionObjectResponseDto?> GetPageAsync(string pageId)
    {
        var response = await _httpClient.GetAsync($"{PagesEndpoint}/{pageId}");
        EnsureSuccess(response);

        return await response.Content.ReadFromJsonAsync<NotionObjectResponseDto>(_jsonOptions);
    }
    
    public async Task<NotionObjectResponseDto?> CreatePageAsync(object pageData)
    {
        var response = await _httpClient.PostAsJsonAsync(PagesEndpoint, pageData);
        EnsureSuccess(response);

        return await response.Content.ReadFromJsonAsync<NotionObjectResponseDto>(_jsonOptions);
    }
    
    public async Task<NotionObjectResponseDto?> UpdatePageAsync(string pageId, object updateData)
    {
        var response = await _httpClient.PatchAsJsonAsync($"{PagesEndpoint}/{pageId}", updateData);
        EnsureSuccess(response);

        return await response.Content.ReadFromJsonAsync<NotionObjectResponseDto>(_jsonOptions);
    }
    
    public async Task MoveToTrashAsync(string pageId)
    {
        var trashData = new { in_trash = true };
        var response = await _httpClient.PatchAsJsonAsync($"{PagesEndpoint}/{pageId}", trashData);
        EnsureSuccess(response);
    }

    private static void EnsureSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = response.Content.ReadAsStringAsync().Result;
            throw new HttpRequestException($"Notion API error: {response.StatusCode} - {error}");
        }
    }
}
