namespace Gig.Framework.Core.DataProviders;

public interface IUnitOfWorkProvider
{
    IUnitOfWork GetUnitOfWork();

    IRequestContext RequestContext();
}