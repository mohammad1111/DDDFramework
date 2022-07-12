using Gig.Framework.Core.FacadServices;
using Gig.Framework.Core.Models;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.User;
using Gig.Sample.Write.Applications.Application.Contracts.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Facades.Facade.Contract.ServiceContract.User
{
    public interface IUserFacadeService : IFacadeService
    {
        Task<GigCommonResult<UserVeiwModel>> CreateUserAsync(CreateUserCommand command);
        Task<GigCommonResult<UserVeiwModel>> UpdateUserAsync(UpdateUserCommand command);
        Task<GigCommonResultBase> RemoveUserAsync(RemoveUserCommand command);
        Task<GigCommonResult<List<UserVeiwModel>>> CreateBulkUsersAsync(CreateBulkUserCommand command);

    }
}
