using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Models;

namespace IISNotionSearch.Application.Interfaces.Services;

public interface IImportService
{
    Task<StandardResponse<List<NotionObjectDto>>> ImportXmlAsync(string xmlContent);
    Task<StandardResponse<List<NotionObjectDto>>> ImportJsonAsync(string jsonContent);
}
