using Gig.Framework.Core.Enums;

namespace Gig.Framework.ReadModel.Models;

public abstract class ViewModel
{
    public long Id { get; set; }

    public byte[] RowVersion { get; set; }

    public StateCodeEnum StateCode { get; set; }
}