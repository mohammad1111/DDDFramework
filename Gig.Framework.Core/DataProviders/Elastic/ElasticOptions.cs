// using System;
// using Gig.Framework.Core.DependencyInjection;
// using Gig.Framework.Core.Settings;
//
// namespace Gig.ManagementSl.Api.FrameworkCode
// {
//     public class ElasticOptions
//     {
//         public bool IndexPerMonth { get; private set; } = true;
//
//         public int AmountOfPreviousIndicesUsedInAlias { get; set; } = 0;
//
//         public Uri ElasticServerUrl { get; private set; } =
//             new(ServiceLocator.Current.Resolve<IDataSetting>().ElasticUrl);
//
//         public string Alias { get; private set; }
//
//         public void UseSettings(string alias)
//         {
//             Alias = alias;
//         }
//
//         public void UseSettings(string alias, Uri elasticServerUrl)
//         {
//             UseSettings(alias);
//             ElasticServerUrl = elasticServerUrl;
//         }
//
//         public void UseSettings(string alias, bool indexPerMonth, int amountOfPreviousIndicesUsedInAlias)
//         {
//             UseSettings(alias);
//             IndexPerMonth = indexPerMonth;
//             AmountOfPreviousIndicesUsedInAlias = amountOfPreviousIndicesUsedInAlias;
//         }
//
//         public void UseSettings(string alias, bool indexPerMonth, int amountOfPreviousIndicesUsedInAlias,
//             Uri elasticServerUrl)
//         {
//             UseSettings(alias, indexPerMonth, amountOfPreviousIndicesUsedInAlias);
//             ElasticServerUrl = elasticServerUrl;
//         }
//     }
// }

