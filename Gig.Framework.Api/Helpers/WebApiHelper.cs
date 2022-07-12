using System.Net.Http.Headers;
using System.Text;
using Gig.Framework.Core;
using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.Api.Helpers;

public class WebApiHelper
{
    private readonly IRequestContext _requestContext;
    private readonly ISerializer _serializer;

    public WebApiHelper(ISerializer serializer, IRequestContext requestContext)
    {
        _serializer = serializer;
        _requestContext = requestContext;
    }

    public static async Task<TResult> PostAsync<TResult>(Uri url, string request, ISerializer serializer,
        IRequestContext requestContext)
    {
        var webApi = new WebApiHelper(serializer, requestContext);
        return await webApi.WebApiCall<TResult>(url, request);
    }

    public async Task<TResult> WebApiCall<TResult>(Uri url, string request)
    {
        using var client = new HttpClient();
        var stringContent = new StringContent(request);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", _requestContext.GetToken());
        var messageBytes = Encoding.Default.GetBytes(request);
        var content = new ByteArrayContent(messageBytes);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var result = await client.PostAsync(url, content);
        if (result.IsSuccessStatusCode)
        {
            var contentResult = await result.Content.ReadAsStringAsync();
            return _serializer.Deserialize<TResult>(contentResult);
        }

        return default;
    }
}