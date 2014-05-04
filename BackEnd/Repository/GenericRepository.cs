using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dal;
using Models;
using System.Data.Entity.Core;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        #region Async Repo
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(TEntity t)
        {
            _dbContext.Set<TEntity>().Add(t);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> RemoveAsync(TEntity t)
        {
            _dbContext.Entry(t).State = EntityState.Deleted;
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(TEntity t)
        {
            _dbContext.Entry(t).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Set<TEntity>().CountAsync();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(match);
        }
        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(match);
        }
        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await _dbContext.Set<TEntity>().Where(match).ToListAsync();
        }
        #endregion
    }
}
