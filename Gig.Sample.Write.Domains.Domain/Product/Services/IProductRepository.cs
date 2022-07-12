using Gig.Framework.Core.DataProviders;
using System.Threading.Tasks;
using Gig.Framework.Domain;

namespace Gig.Sample.Write.Domains.Domain.Product.Services
{
    public interface IProductRepository : IRepository<Product>
    {
        Task AddAsync(Product entity);
        Task RemoveAsync<T>(long id);
    }
}
