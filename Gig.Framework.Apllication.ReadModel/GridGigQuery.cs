using Gig.Framework.ReadModel.Models;

namespace Gig.Framework.Application.ReadModel;

public class GridGigQuery : GigQuery
{
    public GigLoadOptions DataSourceLoadOptions { get; set; } = new()
    {
        Take = int.MaxValue
    };
}