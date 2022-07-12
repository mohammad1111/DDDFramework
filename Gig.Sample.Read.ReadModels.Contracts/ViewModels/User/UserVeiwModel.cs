

using Gig.Framework.ReadModel.Models;
using System;

namespace Gig.Sample.Read.ReadModels.Contracts.ViewModels.User
{
    public class UserVeiwModel : ViewModel
    {
        public long Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool HasAcitve { get; set; }
        public long CompanyId { get; set; }
        public long BranchId { get; set; }
        public long CreatedBy { get; set; }
        public long OwnerId { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
