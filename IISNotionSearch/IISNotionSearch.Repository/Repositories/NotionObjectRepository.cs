using IISNotionSearch.Domain.Entities;
using IISNotionSearch.Domain.Interfaces;
using IISNotionSearch.Repository.Abstractions;

namespace IISNotionSearch.Repository.Repositories;

public class NotionObjectRepository : BaseRepository<NotionObject>, INotionObjectRepository
{
    public NotionObjectRepository(AppDbContext context) : base(context)
    {
    }
}