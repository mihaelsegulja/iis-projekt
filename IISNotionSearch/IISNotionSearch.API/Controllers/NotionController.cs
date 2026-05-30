using IISNotionSearch.API.Abstractions.Attributes;
using IISNotionSearch.API.Abstractions.Controllers;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IISNotionSearch.API.Controllers;

[AuthorizeRoles(Roles.User)]
public class NotionController : BaseController
{
    private readonly INotionService _notionService;

    public NotionController(INotionService notionService)
    {
        _notionService = notionService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query = null)
    {
        var response = await _notionService.SearchAsync(query);
        return HandleResponse(response);
    }

    [HttpGet("pages/{id}")]
    public async Task<IActionResult> GetPage([FromRoute] string id)
    {
        var response = await _notionService.GetPageAsync(id);
        return HandleResponse(response);
    }

    [AuthorizeRoles(Roles.Admin)]
    [HttpPost("pages")]
    public async Task<IActionResult> CreatePage([FromBody] CreateNotionPageDto request)
    {
        var response = await _notionService.CreatePageAsync(request);
        return HandleResponse(response);
    }

    [AuthorizeRoles(Roles.Admin)]
    [HttpPatch("pages/{id}")]
    public async Task<IActionResult> UpdatePage([FromRoute] string id, [FromBody] UpdateNotionPageDto request)
    {
        var response = await _notionService.UpdatePageAsync(id, request);
        return HandleResponse(response);
    }

    [AuthorizeRoles(Roles.Admin)]
    [HttpDelete("pages/{id}")]
    public async Task<IActionResult> DeletePage([FromRoute] string id)
    {
        var response = await _notionService.DeletePageAsync(id);
        return HandleResponse(response);
    }
}