using Gig.Framework.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gig.Sample.Write.Applications.Application.Contracts.User
{
    public class UpdateUserCommand : ICommand
    {
        public long Id { get; set; }
        public long Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool HasAcitve { get; set; }
    }

}
