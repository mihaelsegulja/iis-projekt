using IISNotionSearch.API.Abstractions.Attributes;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Domain.Enums;

namespace IISNotionSearch.API.GraphQL;

[AuthorizeRoles(MinRole = Roles.User)]
public class NotionQuery
{
    public async Task<List<NotionObjectDto>> SearchPages(
        string? query,
        [Service] INotionService notionService)
    {
        var result = await notionService.SearchAsync(query);
        return result.Data?.ToList() ?? [];
    }

    public async Task<NotionObjectDto> GetPage(
        string id,
        [Service] INotionService notionService)
    {
        var result = await notionService.GetPageAsync(id);
        return result.Data ?? throw new GraphQLException(result.Message ?? "Page not found");
    }
}
