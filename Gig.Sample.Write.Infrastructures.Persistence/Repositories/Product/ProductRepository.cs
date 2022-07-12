using Gig.Framework.Core.DataProviders;
using Gig.Framework.Persistence.Ef;
using Gig.Sample.Write.Domains.Domain.Product.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Gig.Sample.Write.Infrastructures.Persistence.Repositories.Product
{
    public class ProductRepository : RepositoryBase<Domains.Domain.Product.Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
                
        }
        protected override IEnumerable<Expression<Func<Domains.Domain.Product.Product, object>>> GetIncludeExpressions()
        {
            return new List<Expression<Func<Domains.Domain.Product.Product, object>>>();
        }
    }
}
