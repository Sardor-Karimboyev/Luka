using System.Linq.Expressions;
using ClassLibrary1;
using Luka.Types;

namespace Luka.Persistence.Postgre.Repositories;

public interface IRepository<TEntity, TIdentifable> where TEntity : IIdentifiable<TIdentifable>
{
    Task<IQueryable<TEntity>> GetAllAsync();
    Task<IQueryable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression);
    Task<TEntity> GetAsync(TIdentifable id);
    Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity, bool>> predicate,
        TQuery query) where TQuery : IPagedQuery;
    Task InsertAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TIdentifable id);
    Task DeleteAsync(Expression<Func<TEntity, bool>> expression);
    Task Save();
}