using Gig.Framework.Core.DataProviders.Convertors;
using Newtonsoft.Json;

namespace Gig.Framework.Core.Models;

public class GigCommonResult<T> : GigCommonResultBase
{
    public T Data { get; set; }

    public static GigCommonResult<T> CreateGigCommonResult(T data, GigCommonResultBase baseData)
    {
        return new GigCommonResult<T>
        {
            DeveloperMessage = baseData.DeveloperMessage,
            FriendlyMessages = baseData.FriendlyMessages,
            HasError = baseData.HasError,
            ValidationErrors = baseData.ValidationErrors,
            Data = data,
            Id = baseData.Id,
            RecId = baseData.RecId
        };
    }

    public static implicit operator bool(GigCommonResult<T> result)
    {
        return !result.HasError;
    }
}

public class GigCommonResultBase
{
    public object RuleResult { get; set; }

    public string DeveloperMessage { get; set; }


    public IList<BusinessErrorMessage> FriendlyMessages { get; set; }

    public bool HasError { get; set; }

    public long Id { get; set; }

    public Guid RecId { get; set; }


    public IDictionary<string, List<BusinessErrorMessage>> ValidationErrors { get; set; }

    public static implicit operator bool(GigCommonResultBase result)
    {
        return !result.HasError;
    }
}