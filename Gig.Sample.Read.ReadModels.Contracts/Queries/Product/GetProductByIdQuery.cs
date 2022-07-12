using Gig.Framework.Application.ReadModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gig.Sample.Read.ReadModels.Contracts.Queries.Product
{
    public class GetProductByIdQuery : QueryCommand
    {
        public long Id { get; set; }
    }
}
