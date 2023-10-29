using Clubhouse.Data.Entities.Base;
using System.Linq.Expressions;

namespace Clubhouse.Data.Repositories.Base;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<bool> AddAsync(TEntity entity,
        bool saveChanges = true,
        CancellationToken ct = default);

    Task<bool> AddAsync(IEnumerable<TEntity> entities,
        bool saveChanges = true,
        CancellationToken ct = default);

    Task<bool> HardDeleteAndSaveChangesAsync(string id, CancellationToken ct);

    Task<bool> HardDeleteAsync(TEntity entity,
        bool saveChanges = false,
        CancellationToken ct = default);

    Task<bool> HardDeleteAsync(IEnumerable<TEntity> entities,
        bool saveChanges = false,
        CancellationToken ct = default);

    Task<bool> SoftDeleteAndSaveChangesAsync(string id,
        CancellationToken ct = default);

    Task<bool> SoftDeleteAsync(string id,
        bool saveChanges = false,
        CancellationToken ct = default);

    Task<PagedResult<TResponse>?> GetAsync<TResponse>(BaseFilter filter,
        CancellationToken ct = default) where TResponse : class;

    Task<TEntity?> GetAsync(string id, CancellationToken ct = default);

    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    Task<IEnumerable<TEntity>?> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<bool> HasDataAsync();

    Task<bool> UpdateAsync(TEntity entity,
        bool saveChanges = true,
        CancellationToken ct = default);

    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}
