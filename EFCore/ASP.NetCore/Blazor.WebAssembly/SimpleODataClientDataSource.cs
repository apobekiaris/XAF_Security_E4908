using System.Collections;
using BlazorWASM;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using Simple.OData.Client;

namespace BlazorWebAssembly; 

public class SimpleODataClientDataSource : GridCustomDataSource {
    private readonly ODataClient _client;

    public SimpleODataClientDataSource(ODataClient client) 
        => _client = client;

    public override async Task<int> GetItemCountAsync(GridCustomDataSourceCountOptions options, CancellationToken cancellationToken) 
        => await ApplyFiltering(options.FilterCriteria, _client.For<Post>()).Count().FindScalarAsync<int>(cancellationToken);

    public override async Task<IList> GetItemsAsync(GridCustomDataSourceItemsOptions options, CancellationToken cancellationToken) {
        var filteredClient = ApplyFiltering(options.FilterCriteria, _client.For<Post>().Top(options.Count).Skip(options.StartIndex));
        var sortedClient = ApplySorting(options, filteredClient);
        return (await sortedClient.FindEntriesAsync(cancellationToken)).ToList();
    }

    private static IBoundClient<Post> ApplyFiltering(CriteriaOperator criteria, IBoundClient<Post> boundClient) 
        => !criteria.ReferenceEqualsNull() ?
            boundClient.Filter(ToSimpleClientCriteria(criteria)) : boundClient;

    private static string ToSimpleClientCriteria(CriteriaOperator criteria) 
        => $"{criteria}".Replace("[", "").Replace("]", "");

    private static IBoundClient<Post> ApplySorting(GridCustomDataSourceItemsOptions options, IBoundClient<Post> boundClient) 
        => options.SortInfo.Any() ? boundClient.OrderBy(options.SortInfo
                .Where(info => !info.DescendingSortOrder).Select(info => info.FieldName).ToArray())
            .OrderByDescending(options.SortInfo
                .Where(info => info.DescendingSortOrder).Select(info => info.FieldName).ToArray()) : boundClient;
}