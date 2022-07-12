using System.Reflection;

namespace Gig.Framework.Scheduling;

public interface IGigScheduler
{
    Task Run(IEnumerable<Assembly> assemblies);
}