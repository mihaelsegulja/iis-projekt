using IISNotionSearch.API.Abstractions.Attributes;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Domain.Enums;

namespace IISNotionSearch.API.GraphQL;

[AuthorizeRoles(MinRole = Roles.Admin)]
public class NotionMutation
{
    public async Task<NotionObjectDto?> CreatePage(
        CreateNotionPageDto input,
        [Service] INotionService notionService)
    {
        var result = await notionService.CreatePageAsync(input);
        if (!result.Success)
            throw new GraphQLException(result.Message ?? "Failed to create page");
        return result.Data;
    }

    public async Task<NotionObjectDto?> UpdatePage(
        string id,
        UpdateNotionPageDto input,
        [Service] INotionService notionService)
    {
        var result = await notionService.UpdatePageAsync(id, input);
        if (!result.Success)
            throw new GraphQLException(result.Message ?? "Failed to update page");
        return result.Data;
    }

    public async Task<bool> DeletePage(
        string id,
        [Service] INotionService notionService)
    {
        var result = await notionService.DeletePageAsync(id);
        return result.Success;
    }
}
