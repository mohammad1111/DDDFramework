using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Gig.Framework.Scheduling;

public interface IGigScheduler
{
    Task Run(IEnumerable<Assembly> assemblies);
}