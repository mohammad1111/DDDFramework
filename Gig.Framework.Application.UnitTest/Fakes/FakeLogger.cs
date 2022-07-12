using System.Threading.Tasks;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class FakeLogger : Framework.Core.Logging.ILogger
    {
        public Task LogAsync<T>(T logData)
        {
            return Task.CompletedTask;
        }
    }
}