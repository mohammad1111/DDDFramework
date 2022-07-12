using Gig.Framework.Core;
using Gig.Framework.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gig.Framework.Persistence.Ef;

public abstract class EntityMapperBase<TEntity> : IEntityMapper
    where TEntity : Entity
{
    private readonly IRequestContext _requestContext;

    public EntityMapperBase(IRequestContext requestContext)
    {
        _requestContext = requestContext;
    }

    public Type MapperType => typeof(TEntity);

    public virtual void Map(ModelBuilder modelBuilder)
    {
    }
}