using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly FacturonDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected BaseRepository(FacturonDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetByConditionAsync(
            Expression<Func<TEntity, bool>> filter,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking()
                .Where(filter)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task PatchAsync(int id, Action<TEntity> patchAction, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
            if (entity == null) return;

            patchAction(entity);
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
            if (entity == null) return;

            entity.Active = false;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
