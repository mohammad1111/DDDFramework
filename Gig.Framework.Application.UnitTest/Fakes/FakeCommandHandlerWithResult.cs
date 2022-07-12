using System.Threading.Tasks;
using Gig.Framework.Core.DataProviders;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class FakeCommandHandlerWithResult : CommandHandler, ICommandHandlerAsync<FakeCommand, FakeCommandResult>
    {

        public FakeCommandHandlerWithResult(IUnitOfWork uow) : base(uow)
        {

        }

        public async Task<FakeCommandResult> HandleAsync(FakeCommand command)
        {

            return new FakeCommandResult { Id = command.Id, IsHandle = true };
        }
    }
}