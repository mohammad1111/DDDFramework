using Gig.Framework.Core.DependencyInjection;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace Gig.Framework.RuleEngine;

public class GigRuleActivator : IRuleActivator
{
    private readonly IGigContainer _scope;

    public GigRuleActivator(IGigContainer scope)
    {
        _scope = scope;
    }

    public IEnumerable<Rule> Activate(Type type)
    {
        yield return (Rule)_scope.ResolveType(type);
    }
}