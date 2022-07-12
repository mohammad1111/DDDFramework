using Gig.Framework.Core.FacadServices;
using Gig.Framework.Core.Models;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product;
using Gig.Sample.Write.Applications.Application.Contracts.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Facades.Facade.Contract.ServiceContract.Product
{
    public interface IProductFacadeService : IFacadeService
    {
        Task<GigCommonResult<ProductVeiwModel>> CreateProductAsync(CreateProductCommand command);
        Task<GigCommonResult<ProductVeiwModel>> UpdateProductAsync(UpdateProductCommand command);
        Task<GigCommonResultBase> RemoveProductAsync(RemoveProductCommand command);
    }
}
