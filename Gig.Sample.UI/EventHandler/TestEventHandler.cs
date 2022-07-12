using System.Threading.Tasks;
using Gig.Sample.UI.Config;
using MassTransit;

namespace Gig.Sample.UI.EventHandler
{
    public class TestEventHandler : IConsumer<ITestEvent>
    {
        private readonly IFakeRegister _fakeRegister;

        public TestEventHandler(IFakeRegister fakeRegister)
        {
            _fakeRegister = fakeRegister;
        }

        public async Task Consume(ConsumeContext<ITestEvent> context)
        {
            var s = _fakeRegister.Id;
        }
    }
}
