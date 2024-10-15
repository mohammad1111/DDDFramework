using System.Threading.Tasks;
using DevExtreme.AspNet.Data.Helpers;
using Gig.Framework.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gig.Framework.Api.Binders;

public class WebApiDataSourceLoadOptionsBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var loadOptions = new WebApiDataSourceLoadOptions
        {
            Take = int.MaxValue
        };

        DataSourceLoadOptionsParser.Parse(loadOptions, key =>
        {
            var value = bindingContext.ValueProvider.GetValue(key);
            if (value != null)
                return value.Values;

            return null;
        });
        bindingContext.Result = ModelBindingResult.Success(loadOptions);
        return Task.CompletedTask;
    }
}