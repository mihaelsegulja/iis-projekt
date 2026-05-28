using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IISNotionSearch.Application.Interfaces.Services;

public interface INotionService
{
    Task<StandardResponse<IEnumerable<NotionObjectDto>>> SearchAsync(string? query = null);
    Task<StandardResponse<NotionObjectDto>> GetPageAsync(string id);
    Task<StandardResponse<NotionObjectDto>> CreatePageAsync(object pageData);
    Task<StandardResponse<NotionObjectDto>> UpdatePageAsync(string id, object pageData);
    Task<StandardResponse<bool>> DeletePageAsync(string id);
}