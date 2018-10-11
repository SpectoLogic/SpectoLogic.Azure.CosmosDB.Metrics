using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SpectoLogic.Azure.CosmosDB.Metrics.CosmosDB;
using SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder.Models;
using SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Pollute;

using MetricModelsDocumentDB = SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models;
using MetricModelsAzureMonitor = SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models;

namespace CosmosDBMetrics
{
    class Program
    {
        private static MetricService _MetricService = null;
        private static IConfigurationRoot Configuration = null;

        private static string SubscriptionId = null;
        private static string TenantId = null;
        private static string ClientId = null;
        private static string ClientSecret = null;
        private static string ResourceGroup = null;
        private static string CosmosDBAccountName = null;
        private static string CosmosDBDatabaseName = null;
        private static string CosmosDBCollectionName = null;
        private static string CosmosDBKey = null;

        public static async Task Main(string[] args)
        {
            InitializeEnvironment();
            ReadConfiguration();

            if (args.Contains("-pollute"))
            {
                await PolluteCosmosDBCollection();
                return;
            }

            _MetricService = new MetricService(
                SubscriptionId,
                TenantId,
                ClientId,
                ClientSecret);

            await RunScenarios();

            return;
        }

        private static void InitializeEnvironment()
        {
            string environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment))
                environment = "Development";
            Console.WriteLine("Environment: {0}", environment);

            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true);
            if (environment == "Development")
            {
                string secretPath = Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}" + "Secrets", Path.DirectorySeparatorChar), $"appsettings.{environment}.json");
                string path = Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar), $"appsettings.{environment}.json");
                if (File.Exists(secretPath)) path = secretPath;

                builder
                    .AddJsonFile(
                        path,
                        optional: true
                    );
            }
            else
            {
                builder
                    .AddJsonFile($"appsettings.{environment}.json", optional: false);
            }

            Configuration = builder.Build();

            // services.AddSingleton<IConfiguration>(Configuration);
            // var serviceProvider = services.BuildServiceProvider();
            // IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
        }

        private static void ReadConfiguration()
        {
            SubscriptionId = Configuration["SubscriptionId"];
            TenantId = Configuration["AADTenant"];

            ClientId = Configuration["ClientId"];
            ClientSecret = Configuration["ClientSecret"];
            ResourceGroup = Configuration.GetSection("CosmosDB")["ResourceGroup"];
            CosmosDBAccountName = Configuration.GetSection("CosmosDB")["AccountName"];
            CosmosDBDatabaseName = Configuration.GetSection("CosmosDB")["DatabaseName"];
            CosmosDBCollectionName = Configuration.GetSection("CosmosDB")["CollectionName"];
            CosmosDBKey = Configuration.GetSection("CosmosDB")["Secret"];
        }

        private static async Task PolluteCosmosDBCollection()
        {
            Console.WriteLine("Polluting given cosmosdb collection with documents.");
            DocDBHelper dbHelp = new DocDBHelper(CosmosDBAccountName, CosmosDBDatabaseName, CosmosDBCollectionName, CosmosDBKey);
            await dbHelp.Connect();
            await dbHelp.AddData();
            return;
        }

        private static async Task RunScenarios()
        {
            await Scenario_GetAzureMonitorMetricDefinitionsForCosmosDBAccount();
            await Scenario_GetCosmosDBMetricDefinitionsForCosmosDBCollection();
            await Scenario_FetchAzureMonitorTotalRequestUnitsForCosmosDBAccount();
            await Scenario_FetchAzureMonitorTotalRequestUnitsForCosmosDBCollection();
            await Scenario_FetchCosmosDBMetricsTotalRequestAvgRPSForCosmosDBCollection();
            await Scenario_FetchCosmosDBMetricAvailableStorageForCosmosDBCollection();
        }

        private static async Task Scenario_GetAzureMonitorMetricDefinitionsForCosmosDBAccount()
        {
            string resourceUri = null;
            Console.WriteLine($"\nFetch Azure Monitor metric definitions for CosmosDB Account: {CosmosDBAccountName}");
            resourceUri = _MetricService.GetResourceUriCosmosDBAccount(ResourceGroup, CosmosDBAccountName);
            var azureMontiorMetricDefinitions = await _MetricService.GetMetricDefinitionsAsync<MetricModelsAzureMonitor.MetricDefinition>(resourceUri);
            PrintMetricsDefinitions(azureMontiorMetricDefinitions);
        }

        private static async Task Scenario_GetCosmosDBMetricDefinitionsForCosmosDBCollection()
        {
            string resourceUri = null;
            Console.WriteLine($"\nFetch CosmosDB metric definitions for CosmosDB Collection: {CosmosDBAccountName}.{CosmosDBDatabaseName}.{CosmosDBCollectionName}");
            resourceUri = _MetricService.GetResourceUriCosmosDBCollection(ResourceGroup, CosmosDBAccountName, CosmosDBKey, CosmosDBDatabaseName, CosmosDBCollectionName);
            var documentDBMetricDefinitions = await _MetricService.GetMetricDefinitionsAsync<MetricModelsDocumentDB.MetricDefinition>(resourceUri);
            PrintMetricsDefinitions(documentDBMetricDefinitions);
        }

        private static async Task Scenario_FetchAzureMonitorTotalRequestUnitsForCosmosDBAccount()
        {
            string resourceUri = null;
            Console.WriteLine($"\nFetch Azure Monitor 'TotalRequestUnits'-metric for CosmosDB Account: {CosmosDBAccountName}");
            resourceUri = _MetricService.GetResourceUriCosmosDBAccount(ResourceGroup, CosmosDBAccountName);
            var metrics = await _MetricService.GetResourceMetricsAsync<MetricModelsAzureMonitor.Metric>(
                resourceUri,
                null,
                (insightsQueryBuilder) =>
                {
                    insightsQueryBuilder
                        .AddMetrics(AzureInsightsMetric.TotalRequestUnits)
                        .AddPeriod(TimeSpan.FromHours(24), MetricInterval.PT5M);
                });
            PrintMetrics<MetricModelsAzureMonitor.Metric>(metrics);
        }

        private static async Task Scenario_FetchAzureMonitorTotalRequestUnitsForCosmosDBCollection()
        {
            string resourceUri = null;
            Console.WriteLine($"\nFetch Azure Monitor 'TotalRequestUnits'-metrics for CosmosDB Collection: {CosmosDBAccountName}.{CosmosDBDatabaseName}.{CosmosDBCollectionName}");
            resourceUri = _MetricService.GetResourceUriCosmosDBAccount(ResourceGroup, CosmosDBAccountName);
            var metrics = await _MetricService.GetResourceMetricsAsync<MetricModelsAzureMonitor.Metric>(
                resourceUri,
                (oDataQueryBuilder) =>
                {
                    oDataQueryBuilder
                        .AddCosmosDBStatusFilter(CosmosDBDatabaseName, CosmosDBCollectionName, "*");
                },
                (insightsQueryBuilder) =>
                {
                    insightsQueryBuilder
                        .AddMetrics(AzureInsightsMetric.TotalRequestUnits)
                        .AddPeriod(TimeSpan.FromHours(24), MetricInterval.PT5M);
                });
            PrintMetrics<MetricModelsAzureMonitor.Metric>(metrics);
        }

        private static async Task Scenario_FetchCosmosDBMetricsTotalRequestAvgRPSForCosmosDBCollection()
        {
            string resourceUri = null;
            Console.WriteLine($"\nFetch CosmosDB metrics 'Total Requests','Average Requests per Second','Throttled Requests','Internal Server Error'  for CosmosDB Collection: {CosmosDBAccountName}.{CosmosDBDatabaseName}.{CosmosDBCollectionName}");
            resourceUri = _MetricService.GetResourceUriCosmosDBCollection(ResourceGroup, CosmosDBAccountName, CosmosDBKey, CosmosDBDatabaseName, CosmosDBCollectionName);
            var docDBmetrics = await _MetricService.GetResourceMetricsAsync<MetricModelsDocumentDB.Metric>(
                resourceUri,
                (oDataQueryBuilder) =>
                {
                    oDataQueryBuilder
                        .AddTime(
                            new DateTime(2018, 10, 10, 03, 53, 30),
                            new DateTime(2018, 10, 10, 13, 53, 30),
                            MetricInterval.PT5M)
                        .And()
                        .AddMetric(
                            CosmosDBMetric.Total_Requests,
                            CosmosDBMetric.Average_Requests_per_Second,
                            CosmosDBMetric.Throttled_Requests,
                            CosmosDBMetric.Internal_Server_Error);
                }, null);
            PrintMetrics<MetricModelsDocumentDB.Metric>(docDBmetrics);
        }

        private static async Task Scenario_FetchCosmosDBMetricAvailableStorageForCosmosDBCollection()
        {
            string resourceUri = null;
            Console.WriteLine($"\nFetch CosmosDB metrics 'Available Storage'  for CosmosDB Collection: {CosmosDBAccountName}.{CosmosDBDatabaseName}.{CosmosDBCollectionName}");
            resourceUri = _MetricService.GetResourceUriCosmosDBCollection(ResourceGroup, CosmosDBAccountName, CosmosDBKey, CosmosDBDatabaseName, CosmosDBCollectionName);
            var docDBmetrics = await _MetricService.GetResourceMetricsAsync<MetricModelsDocumentDB.Metric>(
                resourceUri,
                (oDataQueryBuilder) =>
                {
                    oDataQueryBuilder
                        .AddPeriod(TimeSpan.FromHours(2), MetricInterval.PT5M)
                        .And()
                        .AddMetric(CosmosDBMetric.Available_Storage);
                }, null);
            PrintMetrics<MetricModelsDocumentDB.Metric>(docDBmetrics);
        }

        private static void PrintMetricsDefinitions<T>(IList<T> metricDefinitions)
        {
            if (typeof(T) == typeof(MetricModelsAzureMonitor.MetricDefinition))
            {
                foreach (object def in metricDefinitions)
                {
                    PrintAzureMonitorMetricDefinition(def);
                }
            }
            else if (typeof(T) == typeof(MetricModelsDocumentDB.MetricDefinition))
            {
                foreach (object def in metricDefinitions)
                {
                    PrintDocumentDBMetricDefinition(def);
                }
            }
            else
                Console.WriteLine($"Unsupported type: {typeof(T).FullName}");
        }

        private static void PrintAzureMonitorMetricDefinition(object metricDefinition)
        {
            MetricModelsAzureMonitor.MetricDefinition typeddef = metricDefinition as MetricModelsAzureMonitor.MetricDefinition;
            Console.WriteLine($"Metric-Name: {typeddef.Name.Value}, Aggregation-Type: {typeddef.PrimaryAggregationType?.ToString()} ");
        }

        private static void PrintDocumentDBMetricDefinition(object metricDefinition)
        {
            MetricModelsDocumentDB.MetricDefinition typeddef = metricDefinition as MetricModelsDocumentDB.MetricDefinition;
            Console.WriteLine($"Metric-Name: {typeddef.Name.Value} Aggregation-Type: {typeddef.PrimaryAggregationType?.ToString()} ");
            Console.WriteLine("    Time Grains");
            foreach (var ma in typeddef.MetricAvailabilities)
                Console.WriteLine("        {0}", ma.TimeGrain);
            Console.WriteLine();
        }

        private static void PrintMetrics<T>(IEnumerable<T> metricList)
        {
            if (typeof(T) == typeof(MetricModelsAzureMonitor.Metric))
            {
                foreach (object metric in metricList)
                {
                    PrintAzureMonitorMetric(metric);
                }
            }
            else if (typeof(T) == typeof(MetricModelsDocumentDB.Metric))
            {
                foreach (object metric in metricList)
                {
                    PrintDocumentDBMetric(metric);
                }
            }
            else
                Console.WriteLine($"Unsupported type: {typeof(T).FullName}");
        }

        private static void PrintAzureMonitorMetric(object metric)
        {
            MetricModelsAzureMonitor.Metric typedMetric = metric as MetricModelsAzureMonitor.Metric;

            Console.WriteLine("Metric: {0}", typedMetric.name.Value);
            if (typedMetric.timeseries != null)
                foreach (MetricModelsAzureMonitor.TimeSeries tsv in typedMetric.timeseries)
                {
                    if (tsv.MetadataValues != null)
                    {
                        Console.WriteLine("Timeseries Result for: ");
                        foreach (var metadata in tsv.MetadataValues)
                            Console.WriteLine("{0} = {1} ", metadata.Name.Value, metadata.Value);
                    }
                    foreach (var metricValue in tsv.Data)
                        Console.WriteLine("{0} - {1} - {2}", metricValue.timestamp, metricValue.average, metricValue.total);
                }
            Console.WriteLine();
        }

        private static void PrintDocumentDBMetric(object metric)
        {
            MetricModelsDocumentDB.Metric typedMetric = metric as MetricModelsDocumentDB.Metric;
            Console.WriteLine("Metric: {0}", typedMetric.Name.Value);
            if (typedMetric.MetricValues != null)
                foreach (var metricValue in typedMetric.MetricValues)
                    Console.WriteLine("{0} - {1}", metricValue.Timestamp, metricValue.Average);
            Console.WriteLine();
        }


    }
}
