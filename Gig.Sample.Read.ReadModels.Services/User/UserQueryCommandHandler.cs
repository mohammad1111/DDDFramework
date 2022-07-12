using Gig.Framework.Application.ReadModel;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.ReadModels.Contracts.Queries.User;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Gig.Sample.Read.ReadModels.Services.User
{
    public class UserQueryCommandHandler : CommandQueryHandler
         , IQueryCommandHandlerAsync<GetUserByIdQuery, UserVeiwModel>
         , IQueryCommandHandlerAsync<GetUsersQuery, GigQueryResultViewModel<UserVeiwModel>>
    {
        public UserQueryCommandHandler()
        {
        }
        public async Task<UserVeiwModel> HandleAsync(GetUserByIdQuery command)
        {
            await using var dbContext = new WpapDevContext();
            return await dbContext.Set<ReadModels.User.User>().Where(x => x.Id == command.Id).Select(x => new UserVeiwModel
            {
                Id = x.Id,
                Code = x.code,
                UserName = x.UserName,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                HasAcitve = x.hasAcitve,
                BranchId = x.BranchId,
                CompanyId = x.CompanyId,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                ModifiedBy = x.ModifiedBy,
                ModifiedOn = x.ModifiedOn,
                OwnerId = x.OwnerId,
            }).FirstOrDefaultAsync();
        }
        public async Task<GigQueryResultViewModel<UserVeiwModel>> HandleAsync(GetUsersQuery command)
        {
            await using var dbContext = new WpapDevContext();
            var query = dbContext.Set<ReadModels.User.User>().Select(x => new UserVeiwModel
            {
                Id = x.Id,
                Code = x.code,
                UserName = x.UserName,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                HasAcitve = x.hasAcitve,
                BranchId = x.BranchId,
                CompanyId = x.CompanyId,
                CreatedBy = x.CreatedBy,
                CreatedOn = x.CreatedOn,
                ModifiedBy = x.ModifiedBy,
                ModifiedOn = x.ModifiedOn,
                OwnerId = x.OwnerId,
            }).AsQueryable();

            return await GigQueryResultViewModel<UserVeiwModel>.LoadData(query, command.DataSourceLoadOptions);
        }
    }
}
