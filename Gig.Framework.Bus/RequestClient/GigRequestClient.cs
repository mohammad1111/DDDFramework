using System.Threading.Tasks;
using Gig.Framework.Core;
using Gig.Framework.Core.Events;
using MassTransit;

namespace Gig.Framework.Bus.RequestClient;

public class GigRequestClient<TRequest> : IGigRequestClient<TRequest> where TRequest : class, IEvent
{
    private readonly IRequestClient<TRequest> _client;
    private readonly IRequestContext _requestContext;

    public GigRequestClient(IRequestClient<TRequest> client, IRequestContext requestContext)
    {
        _client = client;
        _requestContext = requestContext;
    }


    public async Task<Response<TResponse>> GetResponse<TResponse>(TRequest request) where TResponse : class
    {
        using var requestClient = _client.Create(request);
        var user = _requestContext.GetUserContext();
        request.BranchId = user.BranchId;
        request.CompanyId = user.BranchId;
        request.UserId = user.BranchId;
        request.LangTypeCode = user.LangTypeCode;
        request.IsAdmin = user.IsAdmin;
        request.SubSystemId = user.SubSystemId;
        return await requestClient.GetResponse<TResponse>();
    }

    public async Task<Response<TResponse, TResponse2>> GetResponse<TResponse, TResponse2>(TRequest request)
        where TResponse : class
        where TResponse2 : class
    {
        using var requestClient = _client.Create(request);
        var user = _requestContext.GetUserContext();
        request.BranchId = user.BranchId;
        request.CompanyId = user.BranchId;
        request.UserId = user.BranchId;
        request.LangTypeCode = user.LangTypeCode;
        request.IsAdmin = user.IsAdmin;
        request.SubSystemId = user.SubSystemId;
        return await _client.GetResponse<TResponse, TResponse2>(request);
    }

    public async Task<Response<TResponse, TResponse2, TResponse3>> GetResponse<TResponse, TResponse2, TResponse3>(
        TRequest request)
        where TResponse : class
        where TResponse2 : class
        where TResponse3 : class
    {
        using var requestClient = _client.Create(request);
        var user = _requestContext.GetUserContext();
        request.BranchId = user.BranchId;
        request.CompanyId = user.BranchId;
        request.UserId = user.BranchId;
        request.LangTypeCode = user.LangTypeCode;
        request.IsAdmin = user.IsAdmin;
        request.SubSystemId = user.SubSystemId;
        return await _client.GetResponse<TResponse, TResponse2, TResponse3>(request);
    }
}