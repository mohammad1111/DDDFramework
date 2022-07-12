using Gig.Framework.Application.ReadModel;
using Gig.Framework.Facade;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.Facade.Contract.Product;
using Gig.Sample.Read.ReadModels.Contracts.Queries.Product;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product;
using System.Threading.Tasks;

namespace Gig.Sample.Read.Facade.Product
{
    public class ProductQueryFacadeService : ReadFacadeService, IProductQueryFacadeService
    {
        public ProductQueryFacadeService(IQueryBus queryBus) : base(queryBus)
        {

        }

        public async Task<GigQueryResultViewModel<ProductVeiwModel>> GetProduct(GetProductQuery queryCommand)
        {
            return await QueryBus.DispatchAsync<GetProductQuery, GigQueryResultViewModel<ProductVeiwModel>>(queryCommand);
        }

        public async Task<ProductVeiwModel> GetProductById(GetProductByIdQuery queryCommand)
        {
            return await QueryBus.DispatchAsync<GetProductByIdQuery, ProductVeiwModel>(queryCommand);
        }
    }
}
