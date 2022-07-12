using System.Threading.Tasks;
using Gig.Framework.Core.DataProviders;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class FakeCommandHandler : CommandHandler, ICommandHandlerAsync<FakeCommand>
    {

        public FakeCommandHandler(IUnitOfWork uow) : base(uow)
        {

        }

        public  Task HandleAsync(FakeCommand command)
        {

            return Task.CompletedTask;
        }
    }
}