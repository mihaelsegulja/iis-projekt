using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Models;

namespace IISNotionSearch.Application.Interfaces.Services;

public interface INotionService
{
    Task<StandardResponse<IEnumerable<NotionObjectDto>>> SearchAsync(string? query = null);
    Task<StandardResponse<NotionObjectDto>> GetPageAsync(string id);
    Task<StandardResponse<NotionObjectDto>> CreatePageAsync(CreateNotionPageDto request);
    Task<StandardResponse<NotionObjectDto>> UpdatePageAsync(string id, UpdateNotionPageDto request);
    Task<StandardResponse<bool>> DeletePageAsync(string id);
}