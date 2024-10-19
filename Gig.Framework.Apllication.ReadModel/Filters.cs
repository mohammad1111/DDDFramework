using System.Linq;

namespace Gig.Framework.Application.ReadModel;

public static class Filters
{
    public static IQueryable<T> SetDynamicFilterAndOrder<T>(this IQueryable<T> query, IQuery command)
    {
        return query; 
    }

}