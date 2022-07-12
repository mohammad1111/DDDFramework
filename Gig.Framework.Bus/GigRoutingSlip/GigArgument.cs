using Gig.Framework.Core;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Bus.GigRoutingSlip;

public class GigArgument : GigEvent
{
    public GigArgument()
    {
    }

    public GigArgument(IRequestContext context) : base(context)
    {
    }
}