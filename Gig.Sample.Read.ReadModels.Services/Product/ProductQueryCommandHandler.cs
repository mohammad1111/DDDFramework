using Gig.Framework.Application.ReadModel;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.ReadModels.Contracts.Queries.Product;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gig.Sample.Read.ReadModels.Services.Product
{
    public class ProductQueryCommandHandler : CommandQueryHandler
         , IQueryCommandHandlerAsync<GetProductByIdQuery, ProductVeiwModel>
         , IQueryCommandHandlerAsync<GetProductQuery, GigQueryResultViewModel<ProductVeiwModel>>
    {
        public async Task<ProductVeiwModel> HandleAsync(GetProductByIdQuery command)
        {
            await using var dbContext = new WpapDevContext();
            return await dbContext.Set<ReadModels.Product.Product>().Where(x => x.Id == command.Id).Select(x => new ProductVeiwModel
            {
                Id = x.Id,
                Count = x.Count,
                Title = x.Title,
                Description =x.Description,
                HasUsed = x.HasUsed,
                Price = x.Price,
                UserId = x.UserId,
                BranchId = x.BranchId,
                CompanyId = x.CompanyId,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                ModifiedBy = x.ModifiedBy,
                ModifiedOn = x.ModifiedOn,
                OwnerId = x.OwnerId,
            }).FirstOrDefaultAsync();
        }

        public async Task<GigQueryResultViewModel<ProductVeiwModel>> HandleAsync(GetProductQuery command)
        {
            await using var dbContext = new WpapDevContext();
            var query = dbContext.Set<ReadModels.Product.Product>().Select(x => new ProductVeiwModel
            {
                Id = x.Id,
                Count = x.Count,
                Title = x.Title,
                Description = x.Description,
                HasUsed = x.HasUsed,
                Price = x.Price,
                UserId = x.UserId,
                BranchId = x.BranchId,
                CompanyId = x.CompanyId,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                ModifiedBy = x.ModifiedBy,
                ModifiedOn = x.ModifiedOn,
                OwnerId = x.OwnerId,
            }).AsQueryable();

            return await GigQueryResultViewModel<ProductVeiwModel>.LoadData(query, command.DataSourceLoadOptions);
        }
    }
}
