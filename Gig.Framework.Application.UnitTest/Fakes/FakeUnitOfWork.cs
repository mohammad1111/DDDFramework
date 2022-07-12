using System.Threading.Tasks;
using Gig.Framework.Core.DataProviders;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public void Begin()
        {

        }

        public Task Commit()
        {
            return Task.CompletedTask;
        }

        public void Rollback()
        {

        }
    }
}