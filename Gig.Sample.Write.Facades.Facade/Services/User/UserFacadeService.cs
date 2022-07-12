using Gig.Framework.Application;
using Gig.Framework.Core.Events;
using Gig.Framework.Core.FacadServices;
using Gig.Framework.Core.Models;
using Gig.Framework.Facade;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.User;
using Gig.Sample.Write.Applications.Application.Contracts.User;
using Gig.Sample.Write.Domains.Domain.Contract.Events;
using Gig.Sample.Write.Facades.Facade.Contract.ServiceContract.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Facades.Facade.Services.User
{
    public class UserFacadeService : FacadeService, IUserFacadeService
    {
        public UserFacadeService(IEventBus eventBus, ICommandBus commandBus) : base(eventBus, commandBus)
        {

        }
        public async Task<GigCommonResult<UserVeiwModel>> CreateUserAsync(CreateUserCommand command)
        {
            long userId = 0;
            EventBus.Subscribe<UserCreatedEvent>(userCreatedEvent =>
            {
                userId = userCreatedEvent.Id;
            });
            var result = await CommandBus.DispatchAsync(command);
            if (!result.HasError)
            {
                result.Id = userId;
            }
            return GigCommonResult<UserVeiwModel>.CreateGigCommonResult(null, result);
        }

        public async Task<GigCommonResult<List<UserVeiwModel>>> CreateBulkUsersAsync(CreateBulkUserCommand command)
        {
           
            var result = await CommandBus.DispatchAsync(command);
            return GigCommonResult<List<UserVeiwModel>>.CreateGigCommonResult(null, result);
        }

        public async Task<GigCommonResultBase> RemoveUserAsync(RemoveUserCommand command)
        {
            return await CommandBus.DispatchAsync(command);
        }

        public async Task<GigCommonResult<UserVeiwModel>> UpdateUserAsync(UpdateUserCommand command)
        {
            var result = await CommandBus.DispatchAsync(command);
            return GigCommonResult<UserVeiwModel>.CreateGigCommonResult(null, result);
        }
    }
}
