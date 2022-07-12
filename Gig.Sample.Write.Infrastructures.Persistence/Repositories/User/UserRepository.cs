using Gig.Framework.Core.DataProviders;
using Gig.Framework.Persistence.Ef;
using Gig.Sample.Write.Domains.Domain.User.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Infrastructures.Persistence.Repositories.User
{
    public class UserRepository : RepositoryBase<Domains.Domain.User.User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override IEnumerable<Expression<Func<Domains.Domain.User.User, object>>> GetIncludeExpressions()
        {
            return new List<Expression<Func<Domains.Domain.User.User, object>>>();
        }
    }
}
