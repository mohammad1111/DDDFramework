using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using Gig.Framework.Core.Models;

namespace Gig.Framework.ReadModel.Models;

public class GigQueryResultViewModel<T> where T : ViewModel
{
    public IEnumerable data { get; set; }


    /// <summary>
    ///     The total number of data objects in the resulting dataset.
    /// </summary>
    [DefaultValue(-1)]
    public int totalCount { get; set; } = -1;

    /// <summary>
    ///     The number of top-level groups in the resulting dataset.
    /// </summary>
    [DefaultValue(-1)]
    public int groupCount { get; set; } = -1;

    public object[] summary { get; set; }

    public string DeveloperMessage { get; set; }
    public IList<BusinessErrorMessage> FriendlyMessages { get; set; }
    public bool HasError { get; set; }

    [Obsolete("This property will be remove as soon as possible...Don't Use it!!!")]
    public long Id { get; set; }

    [Obsolete("This property will be remove as soon as possible...Don't Use it!!!")]
    public Guid RecId { get; set; }

    public IDictionary<string, List<BusinessErrorMessage>> ValidationErrors { get; set; }

    public static async Task<GigQueryResultViewModel<T>> LoadData<TModel>(IQueryable<TModel> query,
        GigLoadOptions dataSource)
    {
        var result = await DataSourceLoader.LoadAsync(query, dataSource);
        return new GigQueryResultViewModel<T>
        {
            data = result.data,
            groupCount = result.groupCount,
            summary = result.summary,
            totalCount = result.totalCount
        };
    }

    public static GigQueryResultViewModel<T> LoadData<TModel>(IEnumerable<TModel> models, GigLoadOptions dataSource)
    {
        var result = DataSourceLoader.Load(models, dataSource);
        return new GigQueryResultViewModel<T>
        {
            data = result.data,
            groupCount = result.groupCount,
            summary = result.summary,
            totalCount = result.totalCount
        };
    }
}