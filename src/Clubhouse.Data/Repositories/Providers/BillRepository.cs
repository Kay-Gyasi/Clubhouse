using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Base;
using Clubhouse.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Data.Repositories.Providers;

[Repository]
public class BillRepository : Repository<Bill>, IBillRepository
{
    public BillRepository(ApplicationDbContext dbContext, 
        ILogger<BillRepository> logger) 
        : base(dbContext, logger)
    {
    }
}