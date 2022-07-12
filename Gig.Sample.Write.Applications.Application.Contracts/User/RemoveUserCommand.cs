using Gig.Framework.Application;

namespace Gig.Sample.Write.Applications.Application.Contracts.User
{
    public class RemoveUserCommand : ICommand
    {
        public RemoveUserCommand(long id)
        {
            Id = id;

        }
        public long Id { get; set; }
    }

}
