using System;
using System.Collections.Generic;
using System.Text;
using Gig.Framework.Application;

namespace Gig.Sample.Write.Applications.Application.Contracts.User
{
    public class CreateBulkUserCommand : ICommand
    {
        public List<CreateUserCommand> CreateUserCommand { get; set; }
    }

}
