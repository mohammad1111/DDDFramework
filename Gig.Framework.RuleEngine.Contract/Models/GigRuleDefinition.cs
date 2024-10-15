using System;
using System.Collections.Generic;

namespace Gig.Framework.RuleEngine.Contract.Models;

public class GigRuleDefinition
{
    public Type RuleType { get; set; }

    public int Priority { get; set; }

    public long SourceRoleId { get; set; }

    public IEnumerable<GigProperty> Properties { get; set; }

    public string Code { get; set; }
}