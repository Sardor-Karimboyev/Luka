using System.Linq.Expressions;
using ClassLibrary1;
using Luka.Types;
using Microsoft.EntityFrameworkCore;

namespace Luka.Persistence.Postgre.Repositories;


internal class Repository<TEntity, TIdentifiable> : IRepository<TEntity, TIdentifiable>
        where TEntity : class, IIdentifiable<TIdentifiable>
    {
        private readonly DbContext _dbContext;

        public Repository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext.Context ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return await Task.FromResult(_dbContext.Set<TEntity>().AsNoTracking());
        }

        public async Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await Task.FromResult(_dbContext.Set<TEntity>().Where(expression).AsNoTracking());
        }

        public async Task<TEntity> GetAsync(TIdentifiable id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate,
            TQuery query) where TQuery : IPagedQuery
        {
            return await _dbContext.Set<TEntity>().Where(predicate).AsNoTracking().PaginateAsync(query);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await Save();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await Save();
        }

        public async Task DeleteAsync(TIdentifiable id)
        {
            var entityToDelete = await GetAsync(id);
            if (entityToDelete != null)
            {
                _dbContext.Set<TEntity>().Remove(entityToDelete);
                await Save();
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            var entitiesToDelete = await _dbContext.Set<TEntity>().Where(expression).ToListAsync();
            if (entitiesToDelete.Any())
            {
                _dbContext.Set<TEntity>().RemoveRange(entitiesToDelete);
                await Save();
            }
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }
    }