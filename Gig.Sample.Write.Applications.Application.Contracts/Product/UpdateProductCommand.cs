using Gig.Framework.Application;

namespace Gig.Sample.Write.Applications.Application.Contracts.Product
{
    public class UpdateProductCommand : ICommand
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool HasUsed { get; set; }
    }

}
