using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Product.Api.Data
{
    public class GenericRepository<TEntity, TContext> : IRepository<TEntity>, IDisposable
        where TEntity : Entities.BaseEntity
        where TContext : DbContext
    {
        private readonly DbSet<TEntity> DbSet;

        private readonly TContext Context;

        private bool disposed;

        public GenericRepository(TContext context)
        {
            this.Context = context;
            this.DbSet = this.Context.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this.DbSet.Where(predicate).AsNoTracking().ToListAsync();
        }

        public virtual async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = this.GetAllIncluding(includeProperties);
            List<TEntity> results = await query.Where(predicate).ToListAsync();
            return results;
        }

        public virtual async Task<TEntity?> GetAsync<TKey>(TKey id)
        {
            return await this.DbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            _ = await this.DbSet.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            // ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            TEntity? existingEntity = await this.DbSet.FindAsync(entity?.Id);

            if (existingEntity != null && entity != null)
            {
                EntityEntry<TEntity> existingEntityEntry = this.DbSet.Entry(existingEntity);
                existingEntityEntry.CurrentValues.SetValues(entity);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            _ = this.DbSet.Remove(entity);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await this.Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }

            this.disposed = true;
        }

        private IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> queryable = this.DbSet.AsNoTracking();
            return includeProperties.Aggregate(queryable, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}
