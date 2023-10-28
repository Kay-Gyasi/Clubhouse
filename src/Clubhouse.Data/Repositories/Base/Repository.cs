using Clubhouse.Data.Entities.Base;
using System.Linq.Expressions;
using Clubhouse.Data.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Clubhouse.Data.Repositories.Base;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger _logger;

    public Repository(ApplicationDbContext dbContext,
        ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected virtual IQueryable<TEntity> GetBaseQuery()
        => _dbContext.Set<TEntity>();

    public async Task<bool> AddAsync(TEntity entity,
        bool saveChanges = true,
        CancellationToken ct = default)
    {
        try
        {
            _ = await _dbContext.AddAsync(entity, ct);
            return await SaveChangesAsync(saveChanges, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding entity to db. {entity}", entity);
            return false;
        }
    }

    public async Task<bool> AddAsync(IEnumerable<TEntity> entities,
        bool saveChanges = true,
        CancellationToken ct = default)
    {
        try
        {
            await _dbContext.AddRangeAsync(entities, ct);
            return await SaveChangesAsync(saveChanges, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding entities to db. {entity}", entities);
            return false;
        }
    }

    public async Task<bool> HardDeleteAndSaveChangesAsync(string id, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogDebug("Invalid id");
                return false;
            }

            var deleted = await _dbContext.Set<TEntity>()
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(ct) > 0;

            if ((!deleted))
            {
                _logger.LogDebug("Could not delete entity with id: {entityId}", id);
                return false;
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting entity with id: {entityId}", id);
            return false;
        }
    }

    public async Task<bool> HardDeleteAsync(TEntity entity,
        bool saveChanges = false,
        CancellationToken ct = default)
    {
        try
        { 
            _ = _dbContext.Set<TEntity>().Remove(entity);
            return await SaveChangesAsync(saveChanges, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting entity. {entity}", entity);
            return false;
        }
    }

    public async Task<bool> HardDeleteAsync(IEnumerable<TEntity> entities,
        bool saveChanges = false,
        CancellationToken ct = default)
    {
        try
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            return await SaveChangesAsync(saveChanges, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting entities. {entities}", entities);
            return false;
        }
    }

    public async Task<bool> SoftDeleteAndSaveChangesAsync(string id,
        CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogDebug("Invalid id");
                return false;
            }

            var deleted = await _dbContext.Set<TEntity>()
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(updates =>
                    updates.SetProperty(p => p.IsDeleted, true), ct) > 0;

            if ((!deleted))
            {
                _logger.LogDebug("Could not delete entity with id: {entityId}", id);
                return false;
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting entity with id: {entityId}", id);
            return false;
        }
    }

    public async Task<bool> SoftDeleteAsync(string id,
        bool saveChanges = false,
        CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogDebug("Invalid id");
                return false;
            }

            var entity = await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null)
            {
                _logger.LogDebug("Entity id passed does not exist in db. {entityId}", id);
                return false;
            }

            entity.IsDeleted = true;
            _ = _dbContext.Set<TEntity>().Update(entity);

            return await SaveChangesAsync(saveChanges, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting entity with id: {entityId}", id);
            return false;
        }
    }

    public virtual async Task<PagedResult<TResponse>?> GetAsync<TResponse>(BaseFilter filter,
        CancellationToken ct = default) where TResponse : class
    {
        try
        {
            var response = await GetBaseQuery()
                .AsNoTracking()
                .Skip(filter.PageIndex * filter.PageSize)
                .Take(filter.PageSize)
                .ProjectToType<TResponse>()
                .ToListAsync(ct);

            var totalCount = await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .LongCountAsync(ct);

            return response
                .ToPagedResult(filter.PageIndex, filter.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving info from db. {filter}", filter);
            return null;
        }
    }

    public async Task<TEntity?> GetAsync(string id, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogDebug("Invalid id");
                return null;
            }

            var entity = await GetBaseQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id, ct);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving info from db. {entityId}", id);
            return null;
        }
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        try
        {
            var entity = await GetBaseQuery().FirstOrDefaultAsync(predicate, ct);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving info from db");
            return null;
        }
    }

    public async Task<IEnumerable<TEntity>?> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var entities = await GetBaseQuery().Where(predicate).ToListAsync();
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving info from db");
            return null;
        }
    }

    public async Task<bool> UpdateAsync(TEntity entity,
        bool saveChanges = true,
        CancellationToken ct = default)
    {
        try
        {
            _ = _dbContext.Update(entity);
            return await SaveChangesAsync(saveChanges, ct);
        }
        catch (Exception)
        {
            _logger.LogDebug("Error while updating entity in db. {entity}", entity);
            return false;
        }
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    private async Task<bool> SaveChangesAsync(bool saveChanges, CancellationToken ct = default)
    {
        if (!saveChanges) return true;
        var isSaved = await _dbContext.SaveChangesAsync(ct) > 0;
        if (!isSaved) _logger.LogDebug("SaveChangesAsync returned 0");
        return isSaved;
    }
}
