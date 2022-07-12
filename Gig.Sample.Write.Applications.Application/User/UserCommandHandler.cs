
using Gig.Framework.Application;
using Gig.Framework.Core.DataProviders;
using Gig.Sample.Write.Applications.Application.Contracts.User;
using Gig.Sample.Write.Domains.Domain.Contract.Events;
using Gig.Sample.Write.Domains.Domain.User.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Applications.Application.User
{
    public class UserCommandHandler : CommandHandler,
        ICommandHandlerAsync<CreateUserCommand>,
        ICommandHandlerAsync<UpdateUserCommand>,
        ICommandHandlerAsync<RemoveUserCommand>,
        ICommandHandlerAsync<CreateBulkUserCommand>
    {
        private readonly IUserRepository _repository;
        public UserCommandHandler(IUnitOfWork uow, IUserRepository repository) : base(uow)
        {
            _repository = repository;
        }
        public async Task HandleAsync(CreateUserCommand command)
        {
            var theUser = new Domains.Domain.User.User()
            {
                Code = command.Code,
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                HasAcitve = command.HasAcitve,
                UserName = command.UserName
            };
            theUser.AddEvent(new UserCreatedEvent());
            await _repository.AddAsync(theUser);
        }

        public async Task HandleAsync(CreateBulkUserCommand command)
        {
            var theUser = new List<Domains.Domain.User.User>();
            foreach (var item in command.CreateUserCommand)
            {
                theUser.Add(new Domains.Domain.User.User()
                {
                    Code = item.Code,
                    Email = item.Email,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    HasAcitve = item.HasAcitve,
                    UserName = item.UserName
                });
            }
            await _repository.BulkInsertAsync(theUser);
        }
        public async Task HandleAsync(UpdateUserCommand command)
        {
            var theUser = await _repository.GetByIdAsync(command.Id);
            theUser.ChangeCode(command.Code);
            theUser.ChangeEmail(command.Email);
            theUser.ChangeFirstName(command.FirstName);
            theUser.ChangeLastName(command.LastName);
            theUser.ChangeUserName(command.UserName);
            theUser.ChangeHasAcitve(command.HasAcitve);
        }
        public async Task HandleAsync(RemoveUserCommand command)
        {
            await _repository.RemoveAsync<Domains.Domain.User.User>(command.Id);
        }
    }
}
