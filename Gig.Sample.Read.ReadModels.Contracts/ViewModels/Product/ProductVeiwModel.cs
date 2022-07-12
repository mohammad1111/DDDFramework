using Gig.Framework.ReadModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gig.Sample.Read.ReadModels.Contracts.ViewModels.Product
{
    public class ProductVeiwModel : ViewModel
    {

        public long UserId { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool HasUsed { get; set; }
        public long CompanyId { get; set; }
        public long BranchId { get; set; }
        public long CreatedBy { get; set; }
        public long OwnerId { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
