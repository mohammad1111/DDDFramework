using System.Reflection;
using Gig.Framework.Core.Exceptions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gig.Framework.Api.Helpers.OpenApi;

public class ExcludePropertyFromOpenApiFilter : ISchemaFilter
{
    public void Apply(
        OpenApiSchema schema,
        SchemaFilterContext context
    )
    {
        if (schema?.Properties == null || schema.Type == null)
        {
            return;
        }

        var excludedProperties = context.Type.GetProperties()
            .Where(
                propertyInfo =>
                    propertyInfo.GetCustomAttribute<GigExcludePropertyFromOpenApiSchemaAttribute>()
                    != null
            );

        foreach (var excludedProperty in excludedProperties)
        {
            var name = MakeFirstLetterLowerCase(excludedProperty.Name);
            if (schema.Properties.ContainsKey(name))
            {
                schema.Properties.Remove(name);
            }
        }
    }

    private string MakeFirstLetterLowerCase(string text)
    {
        return $"{text.First().ToString().ToLower()}{text.Substring(1)}";
    }
}