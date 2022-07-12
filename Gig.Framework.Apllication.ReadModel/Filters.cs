namespace Gig.Framework.Application.ReadModel;

public static class Filters
{
    public static IQueryable<T> SetDynamicFilterAndOrder<T>(this IQueryable<T> query, IQuery command)
    {
        return query; //.WhereClaus(command).OrderByClaus(command);
    }

    //internal static IQueryable<T> WhereClaus<T>(this IQueryable<T> query, IQueryCommand command)
    //{
    //    if (!string.IsNullOrEmpty(command.Where))
    //    {
    //        query = query.OrderBy(command.OrderBy);
    //    }

    //    return query;
    //}

    //internal static IQueryable<T> OrderByClaus<T>(this IQueryable<T> query, IQueryCommand command)
    //{
    //    if (!string.IsNullOrEmpty(command.OrderBy))
    //    {
    //        query = query.OrderBy(command.OrderBy);
    //    }

    //    return query;
    //}
}