using System.Linq.Expressions;
using WestPacificUniversity.EFCore.Entities;

namespace WestPacificUniversity.EFCore.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    Task<bool> ExistsAsync(int id);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<IEnumerable<TEntity>> FindByConditionAsync(
        Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);

    void Add(TEntity entity);
    void Update(TEntity entity);
    Task<bool> HardDeleteAsync(int id);
    void HardDelete(TEntity entity);

    Task<bool> SoftDeleteAsync(int id);
    void SoftDelete(TEntity entity);

    Task<bool> SaveChangesAsync();
}
