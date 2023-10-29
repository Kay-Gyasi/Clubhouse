using Clubhouse.Data.Entities;
using Clubhouse.Data.Repositories.Base;
using Clubhouse.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Data.Repositories.Providers;

[Repository]
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext dbContext, 
        ILogger<RoleRepository> logger) 
        : base(dbContext, logger)
    {
    }
}