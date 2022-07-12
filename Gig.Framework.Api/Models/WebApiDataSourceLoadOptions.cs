using Gig.Framework.Api.Binders;
using Gig.Framework.ReadModel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gig.Framework.Api.Models;

[ModelBinder(typeof(WebApiDataSourceLoadOptionsBinder))]
public class WebApiDataSourceLoadOptions : GigLoadOptions
{
}