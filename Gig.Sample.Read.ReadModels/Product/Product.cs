using Gig.Framework.Core.DataProviders;

namespace Gig.Sample.Read.ReadModels.Product
{
    public class Product : BaseModel
    {
        public long UserId { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool HasUsed { get; set; }
    }
}
