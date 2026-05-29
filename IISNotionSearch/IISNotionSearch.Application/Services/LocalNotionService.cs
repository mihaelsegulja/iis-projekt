using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Mappers;
using IISNotionSearch.Application.Models;
using IISNotionSearch.Domain.Entities;
using IISNotionSearch.Domain.Interfaces;

namespace IISNotionSearch.Application.Services;

public class LocalNotionService : INotionService
{
    private readonly INotionObjectRepository _repository;

    public LocalNotionService(INotionObjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<StandardResponse<IEnumerable<NotionObjectDto>>> SearchAsync(string? query = null)
    {
        var results = string.IsNullOrWhiteSpace(query)
            ? await _repository.FindAsync(x => !x.InTrash)
            : await _repository.FindAsync(x => x.Title.Contains(query) && !x.InTrash);
            
        var dtos = results.Select(r => r.ToDto());
        return StandardResponse<IEnumerable<NotionObjectDto>>.Create(ResultStatus.Ok, dtos);
    }

    public async Task<StandardResponse<NotionObjectDto>> GetPageAsync(string id)
    {
        var results = await _repository.FindAsync(x => x.NotionId == id && !x.InTrash);
        var page = results.FirstOrDefault();
        
        if (page == null)
        {
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.NotFound, message: "Page not found");
        }
        
        return StandardResponse<NotionObjectDto>.Create(ResultStatus.Ok, page.ToDto());
    }

    public async Task<StandardResponse<NotionObjectDto>> CreatePageAsync(CreateNotionPageDto request)
    {
        var entity = new NotionObject
        {
            NotionId = Guid.NewGuid().ToString(),
            ObjectType = "page",
            Title = string.IsNullOrWhiteSpace(request.Title) ? "New Page" : request.Title,
            Url = "local://newpage",
            Icon = request.Icon,
            Cover = request.Cover,
            CreatedTime = DateTimeOffset.UtcNow,
            LastEditedTime = DateTimeOffset.UtcNow,
            InTrash = false
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return StandardResponse<NotionObjectDto>.Create(ResultStatus.Created, entity.ToDto());
    }

    public async Task<StandardResponse<NotionObjectDto>> UpdatePageAsync(string id, UpdateNotionPageDto request)
    {
        var results = await _repository.FindAsync(x => x.NotionId == id && !x.InTrash);
        var page = results.FirstOrDefault();
        
        if (page == null)
            return StandardResponse<NotionObjectDto>.Create(ResultStatus.NotFound, message: "Page not found");

        if (!string.IsNullOrWhiteSpace(request.Title))
            page.Title = request.Title;
        
        if (request.Icon != null)
            page.Icon = request.Icon;
        
        if (request.Cover != null)
            page.Cover = request.Cover;
        
        page.LastEditedTime = DateTimeOffset.UtcNow;
        _repository.Update(page);
        await _repository.SaveChangesAsync();

        return StandardResponse<NotionObjectDto>.Create(ResultStatus.Ok, page.ToDto());
    }

    public async Task<StandardResponse<bool>> DeletePageAsync(string id)
    {
        var results = await _repository.FindAsync(x => x.NotionId == id && !x.InTrash);
        var page = results.FirstOrDefault();
        
        if (page != null)
        {
            page.InTrash = true;
            page.LastEditedTime = DateTimeOffset.UtcNow;
            _repository.Update(page);
            await _repository.SaveChangesAsync();
            return StandardResponse<bool>.Create(ResultStatus.Ok, true);
        }
        
        return StandardResponse<bool>.Create(ResultStatus.NotFound, false, message: "Page not found");
    }
}
