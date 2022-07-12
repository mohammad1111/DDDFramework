using Gig.Framework.Application.ReadModel;
using Gig.Framework.Facade;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.Facade.Contract.User;
using Gig.Sample.Read.ReadModels.Contracts.Queries.User;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.User;
using System.Threading.Tasks;

namespace Gig.Sample.Read.Facade.User
{
    public class UserQueryFacadeService : ReadFacadeService, IUserQueryFacadeService
    {
        public UserQueryFacadeService(IQueryBus queryBus) : base(queryBus)
        {
        }
        public async Task<UserVeiwModel> GetUserById(GetUserByIdQuery queryCommand)
        {
            return await QueryBus.DispatchAsync<GetUserByIdQuery, UserVeiwModel>(queryCommand);
        }
        public async Task<GigQueryResultViewModel<UserVeiwModel>> GetUsers(GetUsersQuery queryCommand)
        {
            return await QueryBus.DispatchAsync<GetUsersQuery, GigQueryResultViewModel<UserVeiwModel>>(queryCommand);
        }
    }
}
