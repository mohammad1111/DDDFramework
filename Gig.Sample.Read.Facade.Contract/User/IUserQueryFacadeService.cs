using Gig.Framework.Core.FacadServices;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.ReadModels.Contracts.Queries.User;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.User;
using System.Threading.Tasks;

namespace Gig.Sample.Read.Facade.Contract.User
{
    public interface IUserQueryFacadeService : IFacadeService
    {
        Task<GigQueryResultViewModel<UserVeiwModel>> GetUsers(GetUsersQuery queryCommand);
        Task<UserVeiwModel> GetUserById(GetUserByIdQuery queryCommand);
    }
}
