using System.Threading.Tasks;
using Gig.Framework.Api.Models;
using Gig.Framework.Core.Models;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.Facade.Contract.Product;
using Gig.Sample.Read.ReadModels.Contracts.Queries.Product;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product;
using Gig.Sample.UI.EventHandler;
using Gig.Sample.Write.Applications.Application.Contracts.Product;
using Gig.Sample.Write.Facades.Facade.Contract.ServiceContract.Product;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Gig.Sample.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductFacadeService _productFacadeService;
        private readonly IProductQueryFacadeService _productQueryFacadeService;
        private readonly IBusControl _control;
        private readonly IBusControl busControl;
        public ProductController(IProductQueryFacadeService productQueryFacadeService,
            IProductFacadeService productFacadeService, IBusControl control, IBusControl publishEndpoint)
        {
            _productQueryFacadeService = productQueryFacadeService;
            _productFacadeService = productFacadeService;
            _control = control;
            busControl = publishEndpoint;
        }


        [HttpGet]
        [Route("Publish")]
        public async Task<string> Get()
        {
            await busControl.Publish<ITestEvent>(new TestEvent());
            return "Ok";
        }

        [HttpGet]
        [Route("{id}")]
        [Route("GetProductTypeById/{id}")]
        public async Task<ProductVeiwModel> GetUserTypeById(long id)
        {
            var result = await _productQueryFacadeService.GetProductById(new GetProductByIdQuery
            {
                Id = id
            });
            return result;
        }

        [HttpGet]
        public async Task<GigQueryResultViewModel<ProductVeiwModel>> GetCustomers(
            [FromQuery] WebApiDataSourceLoadOptions loadOptions)
        {
            return await _productQueryFacadeService.GetProduct(new GetProductQuery
            {
                DataSourceLoadOptions = loadOptions
            });
        }

        [HttpPost]
        public async Task<GigCommonResult<ProductVeiwModel>> CreateAsync(CreateProductCommand command)
        {
            var result = await _productFacadeService.CreateProductAsync(command);
            if (!result.HasError)
            {
                var data = await _productQueryFacadeService.GetProductById(new GetProductByIdQuery
                {
                    Id = result.Id
                });
                result.Data = new ProductVeiwModel
                {
                    Count = data.Count,
                    Title = data.Title,
                    Description = data.Description,
                    HasUsed = data.HasUsed,
                    Price = data.Price,
                    UserId = data.UserId,
                    BranchId = data.BranchId,
                    CompanyId = data.CompanyId,
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn,
                    ModifiedBy = data.ModifiedBy,
                    ModifiedOn = data.ModifiedOn,
                    OwnerId = data.OwnerId
                };
            }

            return result;
        }

        [HttpPut]
        public async Task<GigCommonResult<ProductVeiwModel>> UpdateAsync(UpdateProductCommand command)
        {
            var result = await _productFacadeService.UpdateProductAsync(command);
            if (!result.HasError)
            {
                var data = await _productQueryFacadeService.GetProductById(new GetProductByIdQuery
                {
                    Id = command.Id
                });
                result.Data = new ProductVeiwModel
                {
                    Count = data.Count,
                    Title = data.Title,
                    Description = data.Description,
                    HasUsed = data.HasUsed,
                    Price = data.Price,
                    UserId = data.UserId,
                    BranchId = data.BranchId,
                    CompanyId = data.CompanyId,
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn,
                    ModifiedBy = data.ModifiedBy,
                    ModifiedOn = data.ModifiedOn,
                    OwnerId = data.OwnerId
                };
            }

            return result;
        }

        [HttpDelete]
        public async Task<GigCommonResultBase> RemoveAsync(long id)
        {
            return await _productFacadeService.RemoveProductAsync(new RemoveProductCommand(id));
        }
    }
}