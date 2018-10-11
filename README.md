# SpectoLogic.Azure.CosmosDB.Metrics Library

This library provides a simplified way to access all CosmosDB metrics (from Azure Monitor and CosmosDB Resource Provider) in a single place.

This library has been written as AddOn to the blog article that will be published soon on [https://blog.spectologic.com](https://blog.spectologic.com).

The library comes with a small Console Application which should be pretty self explanatory. You can see some code extracts here:

```csharp
// Example to retrieve Azure Monitor Metric Definitions for ComsmosDB Account
string resourceUri = null;
resourceUri = _MetricService.GetResourceUriCosmosDBAccount(
    ResourceGroup, 
    CosmosDBAccountName);

var azureMontiorMetricDefinitions = await
    _MetricService.GetMetricDefinitionsAsync
    <MetricModelsAzureMonitor.MetricDefinition>(resourceUri);
```

```csharp
// Example to retrieve CosmosDB Metric Definitions for ComsmosDB Collection
string resourceUri = null;
resourceUri = _MetricService.GetResourceUriCosmosDBCollection(
    ResourceGroup, 
    CosmosDBAccountName, 
    CosmosDBKey, 
    CosmosDBDatabaseName, 
    CosmosDBCollectionName);

var documentDBMetricDefinitions = await
    _MetricService.GetMetricDefinitionsAsync
    <MetricModelsDocumentDB.MetricDefinition>(resourceUri);
```

```csharp
// Example to retrieve a Azure Monitor Metric (TotalRequestUnits) 
// for ComsmosDB Collection
string resourceUri = null;

resourceUri = _MetricService.GetResourceUriCosmosDBAccount(
    ResourceGroup, 
    CosmosDBAccountName);

var metrics = await
    _MetricService.GetResourceMetricsAsync<MetricModelsAzureMonitor.Metric>(
        resourceUri,
        (oDataQueryBuilder) =>
        {
            oDataQueryBuilder
                .AddCosmosDBStatusFilter(
                    CosmosDBDatabaseName, 
                    CosmosDBCollectionName, 
                    "*");
        },
        (insightsQueryBuilder) =>
        {
            insightsQueryBuilder
                .AddMetrics(AzureInsightsMetric.TotalRequestUnits)
                .AddPeriod(TimeSpan.FromHours(24), MetricInterval.PT5M);
        });
```