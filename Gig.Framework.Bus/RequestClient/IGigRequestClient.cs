using MassTransit;

namespace Gig.Framework.Bus.RequestClient;

public interface IGigRequestClient<TRequest> where TRequest : class
{
    Task<Response<TResponse>> GetResponse<TResponse>(TRequest request) where TResponse : class;
}