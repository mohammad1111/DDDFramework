using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gig.Framework.Api.Models;
using Gig.Framework.Core.Models;
using Gig.Framework.ReadModel.Models;
using Gig.Sample.Read.Facade.Contract.User;
using Gig.Sample.Read.ReadModels.Contracts.Queries.User;
using Gig.Sample.Read.ReadModels.Contracts.ViewModels.User;
using Gig.Sample.Write.Applications.Application.Contracts.User;
using Gig.Sample.Write.Domains.Domain.User;
using Gig.Sample.Write.Facades.Facade.Contract.ServiceContract.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gig.Sample.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserQueryFacadeService _userQueryFacadeService;
        private readonly IUserFacadeService _userFacadeService;
        public UsersController(IUserQueryFacadeService userQueryFacadeService, IUserFacadeService userFacadeService)
        {
            _userQueryFacadeService = userQueryFacadeService;
            _userFacadeService = userFacadeService;
        }
        [HttpGet]
        [Route("{id}")]
        [Route("GetUserTypeById/{id}")]
        public async Task<UserVeiwModel> GetUserTypeById(long id)
        {
            var result = await _userQueryFacadeService.GetUserById(new GetUserByIdQuery
            {
                Id = id
            });
            return result;
        }
        [HttpGet]
        public async Task<GigQueryResultViewModel<UserVeiwModel>> GetCustomers([FromQuery] WebApiDataSourceLoadOptions loadOptions)
        {
            return await _userQueryFacadeService.GetUsers(new GetUsersQuery()
            {
                DataSourceLoadOptions = loadOptions
            });
        }

        //[HttpPost]
        //public async Task<GigCommonResult<UserVeiwModel>> CreateAsync(CreateUserCommand command)
        //{
        //    var result = await _userFacadeService.CreateUserAsync(command);
        //    if (!result.HasError)
        //    {
        //        var data = await _userQueryFacadeService.GetUserById(new GetUserByIdQuery
        //        {
        //            Id = result.Id
        //        });

        //        result.Data = new UserVeiwModel()
        //        {
        //            Code = data.Code,
        //            UserName = data.UserName,
        //            FirstName = data.FirstName,
        //            LastName = data.LastName,
        //            Email = data.Email,
        //            HasAcitve = data.HasAcitve,
        //            BranchId = data.BranchId,
        //            CompanyId = data.CompanyId,
        //            CreatedBy = data.CreatedBy,
        //            CreatedOn = data.CreatedOn,
        //            ModifiedBy = data.ModifiedBy,
        //            ModifiedOn = data.ModifiedOn,
        //            OwnerId = data.OwnerId,
        //        };
        //    }
        //    return result;
        //}

        [HttpPost]
        public async Task<GigCommonResult<List<UserVeiwModel>>> CreateBulkAsync(CreateBulkUserCommand command)
        {
            var result = await _userFacadeService.CreateBulkUsersAsync(command);
            if (!result.HasError)
            {

            }
            return result;
        }

        [HttpPut]
        public async Task<GigCommonResult<UserVeiwModel>> UpdateAsync(UpdateUserCommand command)
        {
            var result = await _userFacadeService.UpdateUserAsync(command);
            if (!result.HasError)
            {
                var data = await _userQueryFacadeService.GetUserById(new GetUserByIdQuery
                {
                    Id = command.Id
                });
                result.Data = new UserVeiwModel()
                {
                    Code = data.Code,
                    UserName = data.UserName,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = data.Email,
                    HasAcitve = data.HasAcitve,
                    BranchId = data.BranchId,
                    CompanyId = data.CompanyId,
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn,
                    ModifiedBy = data.ModifiedBy,
                    ModifiedOn = data.ModifiedOn,
                    OwnerId = data.OwnerId,
                };
            }
            return result;
        }

        [HttpDelete]
        public async Task<GigCommonResultBase> RemoveAsync(long id)
        {
            return await _userFacadeService.RemoveUserAsync(new RemoveUserCommand(id));
        }
    }
}
