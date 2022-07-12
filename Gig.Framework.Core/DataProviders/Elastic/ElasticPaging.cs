namespace Gig.Framework.Core.DataProviders.Elastic;

public class ElasticPaging
{
    public ElasticPaging(int from, int size)
    {
        From = from;
        Size = size;
    }

    public int From { get; }

    public int Size { get; }

    public static ElasticPaging Default { get; } = new(0, 10);
}