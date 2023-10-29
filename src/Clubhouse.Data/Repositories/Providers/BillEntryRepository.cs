using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Base;
using Clubhouse.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Data.Repositories.Providers;

[Repository]
public class BillEntryRepository : Repository<BillEntry>, IBillEntryRepository
{
    public BillEntryRepository(ApplicationDbContext dbContext, 
        ILogger<BillEntryRepository> logger) 
        : base(dbContext, logger)
    {
    }
}