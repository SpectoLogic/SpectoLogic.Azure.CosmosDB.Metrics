using AML = SpectoLogic.Azure.CosmosDB.Metrics.AzureMonitor.Models;
using DML = SpectoLogic.Azure.CosmosDB.Metrics.DocumentDB.Models;
using SpectoLogic.Azure.CosmosDB.Metrics.RestProviders;
using SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpectoLogic.Azure.CosmosDB.Metrics.CosmosDB
{
    /// <summary>
    /// Author:     Andreas Pollak 
    /// Copyright:  (c) by SpectoLogic
    /// </summary>
    public class MetricService
    {
        private string _subscriptionId = null;
        private string _tenantId = null;
        private string _clientId = null;
        private string _clientSecret = null;
        private string _apiVersion = null;
        private InsightsClientCredentials _credentials;

        private const string C_DocumentDBAccountUriFormat = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DocumentDB/databaseAccounts/{2}";
        private const string C_DocumentDBDatabaseUriFormat = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DocumentDB/databaseAccounts/{2}/databases/{3}";
        private const string C_DocumentDBCollectionUriFormat = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DocumentDB/databaseAccounts/{2}/databases/{3}/collections/{4}";
        private const string C_DocumentDBCollectionRegionPartitionUriFormat = "subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.DocumentDb/databaseAccounts/{2}/region/{3}/databases/{4}/collections/{5}/partitions";

        /// <summary>
        /// Initializes the metric service
        /// </summary>
        /// <param name="subscriptionId">SubscriptionId of subscription to get the metrics from.</param>
        /// <param name="tenantId">Tenant of the Service Principal</param>
        /// <param name="clientId">ApplicationId of the Service Principal</param>
        /// <param name="clientSecret">Secret of Service Prinicipal</param>
        public MetricService(string subscriptionId, string tenantId, string clientId, string clientSecret, string apiVersion = null)
        {
            _subscriptionId = subscriptionId;
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _apiVersion = apiVersion;
            _credentials = new InsightsClientCredentials(_clientId, _clientSecret, _tenantId, apiVersion);
        }

        #region GetResourceUri...

        /// <summary>
        /// ResourceUri Builder for CosmosDBAccount
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public string GetResourceUriCosmosDBAccount(string resourceGroupName, string accountName)
        {
            return string.Format(C_DocumentDBAccountUriFormat, _subscriptionId, resourceGroupName, accountName);
        }
        /// <summary>
        /// ResourceUri Builder for CosmosDBDatabase
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="accountName"></param>
        /// <param name="accessKey"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public string GetResourceUriCosmosDBDatabase(string resourceGroupName, string accountName, string accessKey, string databaseName)
        {
            var result = AzureBaseInsightsProvider.GetResourceIDs(accountName, accessKey, databaseName, null);
            return string.Format(C_DocumentDBDatabaseUriFormat, _subscriptionId, resourceGroupName, accountName, result.databaseResourceId);
        }
        /// <summary>
        /// ResourceUri Builder for CosmosDBCollection
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="accountName"></param>
        /// <param name="accessKey"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public string GetResourceUriCosmosDBCollection(string resourceGroupName, string accountName, string accessKey, string databaseName, string collectionName)
        {
            var result = AzureBaseInsightsProvider.GetResourceIDs(accountName, accessKey, databaseName, collectionName);
            return string.Format(C_DocumentDBCollectionUriFormat, _subscriptionId, resourceGroupName, accountName, result.databaseResourceId, result.collectionResourceId);
        }

        /// <summary>
        /// ResourceUri Builder for CosmosDBCollection with Partitions
        /// </summary>
        /// <param name="resourceGroupName"></param>
        /// <param name="accountName"></param>
        /// <param name="accessKey"></param>
        /// <param name="region"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public string GetResourceUriCosmosDBCollectionPartitions(string resourceGroupName, string accountName, string accessKey, string region, string databaseName, string collectionName)
        {
            var result = AzureBaseInsightsProvider.GetResourceIDs(accountName, accessKey, databaseName, collectionName);
            return string.Format(C_DocumentDBCollectionRegionPartitionUriFormat, _subscriptionId, resourceGroupName, accountName, region, result.databaseResourceId, result.collectionResourceId);
        }

        #endregion

        /// <summary>
        /// Get a specific Resource Metric
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <param name="oDataQueryBuilderDelegate">Delegate to an inline lambda function to build a simple oDataQuery with a Builder class.</param>
        /// <param name="insightsQueryBuilderDelegate">Delegate to an inline lambda function to create the query parameters for an Azure Monitor Query with a Builder class</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetResourceMetricsAsync<T>(
            string resourceUri,
            Action<ODataQueryBuilder> oDataQueryBuilderDelegate,
            Action<InsightsQueryBuilder> insightsQueryBuilderDelegate)
        {
            AzureBaseInsightsProvider provider = CreateProvider<T>();
            var jsonMetrics = await provider.GetMetricAsJSONAsync(
                                resourceUri,
                                (queryBuilder) =>
                                {
                                    oDataQueryBuilderDelegate?.Invoke(queryBuilder);
                                },
                                insightsQueryBuilderDelegate: (iQueryBuilder) =>
                                {
                                    insightsQueryBuilderDelegate?.Invoke(iQueryBuilder);
                                }
                                );
            return DeserializeJson<T>(jsonMetrics);
        }

        public async Task<IList<T>> GetMetricDefinitionsAsync<T>(string resourceUri)
        {
            AzureBaseInsightsProvider provider = CreateProvider<T>();
            var jsonDefinitions = await provider.GetMetricDefinitionsAsJSONAsync(resourceUri);
            return DeserializeJson<T>(jsonDefinitions);
        }

        /// <summary>
        /// Creates the correct Provider depending on the type parameter.
        /// Throws an exception if the type is not known!
        /// </summary>
        /// <typeparam name="T">
        ///     Type Metric or MetricDefinition 
        ///     from one of the following namespaces: 
        ///         - CosmosDB.Metrics.AzureMonitor.Models
        ///         - CosmosDB.Metrics.DocumentDB.Models
        /// </typeparam>
        /// <returns>AzureBaseInsightsProvider</returns>
        private AzureBaseInsightsProvider CreateProvider<T>()
        {
            AzureBaseInsightsProvider provider = null;
            if ((typeof(T) == typeof(DML.MetricDefinition)) || (typeof(T) == typeof(DML.Metric)))
            {
                provider = new CosmosDBInsightsProvider(_credentials);
            }
            else
            if ((typeof(T) == typeof(AML.MetricDefinition)) || (typeof(T) == typeof(AML.Metric)))
            {
                provider = new AzureMonitorInsightsProvider(_credentials);
            }
            else
                throw new Exception($"Unknown Type: {typeof(T).FullName}");
            return provider;
        }

        /// <summary>
        /// Deserializes the given JSON to a list of requested objects
        /// </summary>
        /// <typeparam name="T">
        ///     Type Metric or MetricDefinition 
        ///     from one of the following namespaces: 
        ///         - CosmosDB.Metrics.AzureMonitor.Models
        ///         - CosmosDB.Metrics.DocumentDB.Models
        /// </typeparam>
        /// <param name="json">Input JSON as the internal HTTP-REQUEST return. In some cases some meta data is wrapped around the list.</param>
        /// <returns>List of the requested objects</returns>
        private IList<T> DeserializeJson<T>(string json)
        {
            object result = null;

            if (typeof(T) == typeof(DML.MetricDefinition))
            {
                DML.MetricDefinitionCollection definition = JsonConvert.DeserializeObject<DML.MetricDefinitionCollection>(json);
                result = (IList<DML.MetricDefinition>)definition.Value;
            }
            else if (typeof(T) == typeof(AML.MetricDefinition))
            {
                result = (IList<AML.MetricDefinition>)JsonConvert.DeserializeObject<AML.MetricDefinitionResult>(json).Value;
            }
            else if (typeof(T) == typeof(DML.Metric))
            {
                // Use CosmosDB Provider
                DML.MetricCollection metrics = JsonConvert.DeserializeObject<DML.MetricCollection>(json);
                result = (IList<DML.Metric>)metrics.Value;
            }
            else if (typeof(T) == typeof(AML.Metric))
            {
                // Use Insights Provider
                AML.MetricResult restResult = JsonConvert.DeserializeObject<AML.MetricResult>(json);
                result = (List<AML.Metric>)restResult.Value;
            }
            else
                throw new Exception($"Unknown Type: {typeof(T).FullName}");
            return (IList<T>)result;
        }

        //private IEnumerable<Microsoft.Azure.Insights.Models.MetricDefinition> GetMetricDefinitionsAzureInsightsSDK(string resourceUri)
        //{
        //    using (var client = new Microsoft.Azure.Insights.InsightsClient(_credentials))
        //    {
        //        client.SubscriptionId = _subscriptionId;
        //        return Microsoft.Azure.Insights.MetricDefinitionsOperationsExtensions.List(client.MetricDefinitions,resourceUri, null);
        //    }
        //}
        //private IEnumerable<Microsoft.Azure.Insights.Models.Metric> GetResourceMetricsAzureInsightsSDK(
        //   string resourceUri,
        //   Action<ODataQueryBuilder> oDataQueryBuilderDelegate)
        //{
        //    ODataQueryBuilder queryBuilder = new ODataQueryBuilder();
        //    oDataQueryBuilderDelegate(queryBuilder);
        //    using (var client = new Microsoft.Azure.Insights.InsightsClient(_credentials))
        //    {
        //        client.SubscriptionId = _subscriptionId;
        //        return Microsoft.Azure.Insights.MetricsOperationsExtensions.List(client.Metrics, resourceUri, queryBuilder.ToString());
        //    }
        //}
    }
}
