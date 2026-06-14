using IISNotionSearch.API.Abstractions.Attributes;
using IISNotionSearch.API.Abstractions.Controllers;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IISNotionSearch.API.Controllers;

[AuthorizeRoles(MinRole = Roles.Admin)]
public class ImportController : BaseController
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost]
    [Consumes("application/json", "application/xml")]
    public async Task<IActionResult> Import()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        
        if (string.IsNullOrWhiteSpace(body))
            return BadRequest(new { message = "Request body cannot be empty." });
        
        var contentType = Request.ContentType?.ToLowerInvariant();

        if (contentType?.Contains("xml") == true)
            return HandleResponse(await _importService.ImportXmlAsync(body));

        if (contentType?.Contains("json") == true)
            return HandleResponse(await _importService.ImportJsonAsync(body));

        return BadRequest(new
        {
            message = "Unsupported content type. Use application/xml or application/json."
        });
    }
}