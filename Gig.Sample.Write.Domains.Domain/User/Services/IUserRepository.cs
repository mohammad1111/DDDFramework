using Gig.Framework.Core.DataProviders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gig.Framework.Domain;

namespace Gig.Sample.Write.Domains.Domain.User.Services
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddAsync(User entity);
        Task RemoveAsync<T>(long id);
        Task BulkInsertAsync(IList<User> entites);
    }
}
