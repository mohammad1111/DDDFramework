using System;
using System.Collections.Generic;

namespace Gig.Framework.RuleEngine.Contract.Models;

public class GigRuleSetModel
{
    public GigRuleSetModel(Guid recGuid, string tag, object model, IEnumerable<GigRuleDefinition> rules)
    {
        RecGuid = recGuid;
        Tag = tag;
        Model = model;
        Rules = rules;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public Guid RecGuid { get; }

    public string Tag { get; }

    public object Model { get; }

    public IEnumerable<GigRuleDefinition> Rules { get; }
}