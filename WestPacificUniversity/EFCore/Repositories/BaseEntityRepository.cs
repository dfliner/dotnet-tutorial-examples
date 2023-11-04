using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WestPacificUniversity.EFCore.Entities;
using WestPacificUniversity.Utilities;

namespace WestPacificUniversity.EFCore.Repositories;

public class BaseEntityRepository<TEntity> : IRepository<TEntity>
    where TEntity : AbstractEntityBase
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<TEntity> _entities;

    public BaseEntityRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _entities = dbContext.Set<TEntity>();
    }

    protected IQueryable<TEntity> GetAllAsQueryable()
    {
        return _entities.AsQueryable();
    }

    public virtual Task<bool> ExistsAsync(int id)
    {
        return _entities.AnyAsync(x => x.Id == id);
    }

    public virtual Task<TEntity?> GetByIdAsync(int id)
    {
        return _entities.SingleOrDefaultAsync(e => e.Id == id);
    }

    public virtual Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        CheckArgument.ThrowIfNull(predicate);

        return _entities.FirstOrDefaultAsync(predicate);
    }

    public virtual Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        CheckArgument.ThrowIfNull(predicate);

        return _entities.SingleOrDefaultAsync(predicate);
    }

    public virtual async Task<IEnumerable<TEntity>> FindByConditionAsync(
        Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        CheckArgument.ThrowIfNull(filter);

        var query = _entities.AsQueryable();
        query = query.Where(filter);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public virtual void Add(TEntity entity)
    {
        _entities.Add(entity);
    }

    public virtual void Update(TEntity entity)
    {
        // _entities.Update(entity);

        _entities.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public virtual async Task<bool> HardDeleteAsync(int id)
    {
        TEntity? entity = await _entities.FindAsync(id);
        if (entity != null)
        {
            HardDelete(entity);
            return true;
        }
        return false;
    }

    public virtual void HardDelete(TEntity entity)
    {
        if (_dbContext.Entry(entity).State == EntityState.Detached)
        {
            _entities.Attach(entity);
        }
        _entities.Remove(entity);
    }

    public virtual async Task<bool> SoftDeleteAsync(int id)
    {
        TEntity? entity = await _entities.FindAsync(id);
        if (entity != null)
        {
            SoftDelete(entity);
            return true;
        }
        return false;
    }

    public virtual void SoftDelete(TEntity entity)
    {
        var entry = _dbContext.ChangeTracker
            .Entries()
            .FirstOrDefault(entry => entry.Entity == entity);

        if (entry != null)
        {
            entity.IsDeleted = true;
        }
    }

    public virtual async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
