using Gig.Sample.Write.Domains.Domain.User.Services;

namespace Gig.Sample.Write.Domains.Domain.Service.User
{
    public class UserDomainService : IUserDomainService
    {
        private readonly IUserRepository _userRepository;
        public UserDomainService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
