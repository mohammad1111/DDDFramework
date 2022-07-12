using Gig.Framework.Application.ReadModel;

namespace Gig.Sample.Read.ReadModels.Contracts.Queries.User
{
    public class GetUserByIdQuery : QueryCommand
    {
        public long Id { get; set; }
    }
}
