using Gig.Framework.Core.Exceptions;
using Newtonsoft.Json;

namespace Gig.Framework.Core.Models;

public class BusinessErrorMessage
{
    private Exception _exception;

    public string ErrorCode { get; set; }

    public string Message { get; set; }

    [JsonIgnore]
    [GigExcludePropertyFromOpenApiSchema]
    public Exception Exception
    {
        get => _exception;
        set
        {
            _exception = value;
            if (value != null) DevelopMessage = value.ToString();
        }
    }


    public string DevelopMessage { get; set; }
}