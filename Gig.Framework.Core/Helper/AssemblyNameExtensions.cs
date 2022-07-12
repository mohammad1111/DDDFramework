using System.Reflection;

namespace Gig.Framework.Core.Helper;

public static class AssemblyNameExtensions
{
    public static IEnumerable<AssemblyName> GetReferencesAssemblies(
        this IEnumerable<AssemblyName> assemblies)
    {
        return assemblies.Select(Assembly.Load)
            .SelectMany((Func<Assembly, IEnumerable<AssemblyName>>)(r => r.GetReferencedAssemblies()));
    }
}