using Gig.Framework.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gig.Sample.Write.Domains.Domain.Product
{
    public class Product : AggregateRoot
    {
        public Product()
        {

        }


        public Product(Guid id) : base(id)
        {

        }

        public Product(long userId, int count, string title, string description, decimal price, bool hasUsed)
        {
            UserId = userId;
            Count = count;
            Title = title;
            Description = description;
            Price = price;
            HasUsed = hasUsed;
        }
        public long UserId { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool HasUsed { get; set; }

        public void ChangeUserId(long userId)
        {
            UserId = userId;
        }
        public void ChangeCount(int count)
        {
            Count = count;
        }
        public void ChangeTitle(string title)
        {
            Title = title;
        }
        public void ChangeDescription(string description)
        {
            Description = description;
        }
        public void ChangePrice(decimal price)
        {
            Price = price;
        }
        public void ChangeHasUsed(bool hasUsed)
        {
            HasUsed = hasUsed;
        }
    }
}
