using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Exceptions;
using Gig.Framework.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gig.Framework.Persistence.Ef;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly GigDbContext _gigDbContext;
    private readonly IRequestContext _requestContext;

    protected RepositoryBase(GigDbContext gigDbContext,IRequestContext requestContext)
    {
        _gigDbContext = gigDbContext;
        _requestContext = requestContext;
    }


    public virtual async Task<TEntity> GetByIdAsync(long id)
    {
        var aggregateQuery = ConvertToAggregate(Set().AsQueryable());

        var result = await aggregateQuery.FirstOrDefaultAsync(x => x.Id == id);

        if (result == null) throw new FrameworkException($"داده ای با شناسه {id} و سطح دسترسی شما پیدا نشد ");

        return result;
    }


    
    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<long> ids)
    {
        var aggregateQuery = ConvertToAggregate(Set().AsQueryable());
        var result = await aggregateQuery.Where(x => ids.Contains(x.Id)).ToListAsync();
        if (result.Any()) throw new Exception("اطلاعات پیدا نشد");

        return result;
    }

    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<IdRowVersionPair> idRowVersionPairs)
    {
        var aggregateQuery = ConvertToAggregate(Set().AsQueryable());
        var result = await aggregateQuery
            .Where(x => idRowVersionPairs.Any(y => y.Id == x.Id && y.RowVersion == x.RowVersion)).ToListAsync();
        if (result.Any()) throw new Exception("اطلاعات پیدا نشد");

        if (result.Any(x => idRowVersionPairs.Any(y => x.Id == y.Id && x.RowVersion != y.RowVersion)))
            throw new Exception("داده های درخواستی قبلا تغییر کرده، لطفا یک بار دیگر اطلاعات را بارگذاری نمایید");

        return result;
    }



    public DbSet<TEntity> ContextDbSet()
    {
        return _gigDbContext.Set<TEntity>();
    }

    public IQueryable<T> Set<T>() where T : Entity
    {
        return _gigDbContext.Set<T>().Where(x =>
            !x.IsDeleted && (x.CompanyId == _requestContext.GetUserContext().CompanyId ||
                             x.CompanyId == 100));
    }


    public IQueryable<TEntity> Set()
    {
        return _gigDbContext.Set<TEntity>().Where(x =>
            !x.IsDeleted && (x.CompanyId == _requestContext.GetUserContext().CompanyId ||
                             x.CompanyId == 100));
    }

    public async Task AddAsync(TEntity entity)
    {
        await _gigDbContext.AddAsync(entity);
    }

    public async Task AddRangeAsync(IList<TEntity> entities)
    {
        await _gigDbContext.AddRangeAsync(entities);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _gigDbContext.Update(entity);
    }

    public async Task RemoveAsync<T>(long id)
    {
        var entity = await GetByIdAsync(id);
        _gigDbContext.Set<TEntity>().Remove(entity);
    }

    public async Task RemoveAsync(TEntity entity)
    {
        await Task.Factory.StartNew(() =>
        {
            _gigDbContext.Set<TEntity>().Attach(entity);
            _gigDbContext.Set<TEntity>().Remove(entity);
        });
    }

    public IQueryable<TEntity> ConvertToAggregate(IQueryable<TEntity> set)
    {
        var result = set;
        var includeExpressions = GetIncludeExpressions();
        var user = _requestContext.GetUserContext();
        if (includeExpressions != null)
            foreach (var expression in includeExpressions)
                result = result.Include(expression)
                    .Where(x => !x.IsDeleted && (x.CompanyId == user.CompanyId || x.CompanyId == 100));

        return result;
    }

    protected abstract IEnumerable<Expression<Func<TEntity, object>>> GetIncludeExpressions();
}