using Gig.Framework.Core.FacadServices;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.ReadModels.Contracts.Queries.Product;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product;
using System.Threading.Tasks;

namespace Gig.Sample.Read.Facade.Contract.Product
{
    public interface IProductQueryFacadeService : IFacadeService
    {
        Task<GigQueryResultViewModel<ProductVeiwModel>> GetProduct(GetProductQuery queryCommand);
        Task<ProductVeiwModel> GetProductById(GetProductByIdQuery queryCommand);
    }
}
