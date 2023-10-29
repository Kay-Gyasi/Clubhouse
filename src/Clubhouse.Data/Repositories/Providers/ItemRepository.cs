using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Base;
using Clubhouse.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Data.Repositories.Providers;

[Repository]
public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext dbContext, 
        ILogger<ItemRepository> logger) 
        : base(dbContext, logger)
    {
    }
}