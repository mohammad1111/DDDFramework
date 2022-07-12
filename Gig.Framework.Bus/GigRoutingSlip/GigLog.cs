using Gig.Framework.Core;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Bus.GigRoutingSlip;

public class GigLog : GigEvent
{
    public GigLog()
    {
    }

    public GigLog(IRequestContext context) : base(context)
    {
    }
}