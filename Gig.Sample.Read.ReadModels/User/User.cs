using Gig.Framework.Core.DataProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gig.Sample.Read.ReadModels.User
{
    public class User : BaseModel
    {
        public long code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool hasAcitve { get; set; }
    }
}
