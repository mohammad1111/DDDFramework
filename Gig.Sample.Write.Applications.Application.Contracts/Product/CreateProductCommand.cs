using Gig.Framework.Application;

namespace Gig.Sample.Write.Applications.Application.Contracts.Product
{
    public class CreateProductCommand : ICommand
    {
        public long UserId { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool HasUsed { get; set; }
    }
}
