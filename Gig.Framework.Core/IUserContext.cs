namespace Gig.Framework.Core;

public interface IUserContext
{
    DateTime Created { get; }


    Guid TraceId { get; }

    string DisplayName { get; }


    long UserId { get; }


    long CompanyId { get; }


    long BranchId { get; }

    int LangTypeCode { get; }
    long SubSystemId { get; }

    int[] Operations { get; }


    List<long> UserCompanies { get; }


    bool IsAdmin { get; }

 
}