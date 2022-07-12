namespace Gig.Framework.Domain.IdGenerators;

public interface IGigIdGenerator
{
    long NewId();

    IEnumerable<long> Take(int count);
}