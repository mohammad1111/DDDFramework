using Gig.Framework.Application;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.Models;
using Gig.Framework.Facade;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product;
using Gig.Sample.Write.Applications.Application.Contracts.Product;
using Gig.Sample.Write.Domains.Domain.Contract.Events;
using Gig.Sample.Write.Facades.Facade.Contract.ServiceContract.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Facades.Facade.Services.Product
{
    public class ProductFacadeService : FacadeService, IProductFacadeService
    {
        public ProductFacadeService(IEventBus eventBus, ICommandBus commandBus) : base(eventBus, commandBus)
        {

        }
        public async Task<GigCommonResult<ProductVeiwModel>> CreateProductAsync(CreateProductCommand command)
        {
            long productId = 0;
            EventBus.Subscribe<ProductCreatedEvent>(productCreatedEvent =>
            {
                productId = productCreatedEvent.Id;
            });
            var result = await CommandBus.DispatchAsync(command);
            if (!result.HasError)
            {
                result.Id = productId;
            }
            return GigCommonResult<ProductVeiwModel>.CreateGigCommonResult(null, result);
        }
        public async Task<GigCommonResultBase> RemoveProductAsync(RemoveProductCommand command)
        {
            return await CommandBus.DispatchAsync(command);
        }
        public async Task<GigCommonResult<ProductVeiwModel>> UpdateProductAsync(UpdateProductCommand command)
        {
            var result = await CommandBus.DispatchAsync(command);
            return GigCommonResult<ProductVeiwModel>.CreateGigCommonResult(null, result);
        }
    }
}
