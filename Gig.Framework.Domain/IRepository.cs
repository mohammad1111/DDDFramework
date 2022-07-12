using Gig.Framework.Core.DataProviders;

namespace Gig.Framework.Domain;

public interface IRepository<TEntity>:IRepository where TEntity : Entity
{
    Task<TEntity> GetByIdAsync(long id);

    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<long> ids);

    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<IdRowVersionPair> idRowVersionPairs);
}