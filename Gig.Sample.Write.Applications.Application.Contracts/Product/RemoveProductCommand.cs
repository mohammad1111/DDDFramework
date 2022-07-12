using Gig.Framework.Application;

namespace Gig.Sample.Write.Applications.Application.Contracts.Product
{
    public class RemoveProductCommand : ICommand
    {
        public RemoveProductCommand(long id)
        {
            Id = id;

        }
        public long Id { get; set; }
    }

}
