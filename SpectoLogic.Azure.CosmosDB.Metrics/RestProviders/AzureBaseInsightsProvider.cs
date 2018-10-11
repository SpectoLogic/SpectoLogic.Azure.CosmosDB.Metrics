using Microsoft.Rest;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Text;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq;
using SpectoLogic.Azure.CosmosDB.Metrics.RestProviders.Builder;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders
{
    /// <summary>
    /// Inherits from Microsoft.Rest.ServiceClient
    /// Author:     Andreas Pollak 
    /// Copyright:  (c) by SpectoLogic
    /// </summary>
    public abstract class AzureBaseInsightsProvider : ServiceClient<AzureBaseInsightsProvider>
    {
        ServiceClientCredentials _credentials = null;

        /// <summary>
        /// Initialize AzureBaseInsightsProvider HttpClient!
        /// </summary>
        /// <param name="credentials">ServiceClientCredentials to use for authentication. </param>
        public AzureBaseInsightsProvider(ServiceClientCredentials credentials)
        {
            this._credentials = credentials;
            this._credentials.InitializeServiceClient<AzureBaseInsightsProvider>(this);
            this.InitializeHttpClient(new AzureBaseInsightsHttpHandler(_credentials));
        }

        /// <summary>
        /// This method must be implemented by the inheriting classes to provide the correct base URI to the service:
        ///     Azure Monitor and CosmosDB Resource Provider have different REST-EndPoints.
        /// </summary>
        /// <param name="resourceUri">Resource URI where we want to get metrics from</param>
        /// <param name="action">Action: metrics, metricdefinitions</param>
        /// <param name="apiVersion"></param>
        /// <returns></returns>
        abstract protected StringBuilder CreateBaseRequestUri(string resourceUri, string action, string apiVersion = null);

        /// <summary>
        /// Constructs the base URI "https://management.azure.com/{resourceuri}/"
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <returns></returns>
        protected StringBuilder CreateResourceRequestUri(string resourceUri)
        {
            StringBuilder requestUri = new StringBuilder("https://management.azure.com");
            if (!resourceUri.StartsWith("/"))
                requestUri.Append("/");

            requestUri.Append(resourceUri);

            if (!resourceUri.EndsWith("/"))
                requestUri.Append("/");
            return requestUri;
        }

        #region CosmosDB Helpers to retrieve ResourceIds of Collection and Database

        /// <summary>
        /// Retrieves the internal resourceIds of a CosmosDB database and collection
        /// </summary>
        /// <param name="docDBAccount"></param>
        /// <param name="docDBKey"></param>
        /// <param name="docDBName"></param>
        /// <param name="docDBCollectionName"></param>
        /// <returns>A tuple with database and collection resourceIds</returns>
        public static (string databaseResourceId, string collectionResourceId) GetResourceIDs(
            string docDBAccount, 
            string docDBKey, 
            string docDBName, 
            string docDBCollectionName)
        {
            string databaseResourceId = null;
            string collectionResourceId = null;

            var client = new DocumentClient(new Uri($"https://{docDBAccount}.documents.azure.com:443/"), docDBKey);
            Database database = FindDocumentDBDatabaseByName(docDBName, client);
            if (database != null)
            {
                databaseResourceId = database.ResourceId;

                if (!string.IsNullOrEmpty(docDBCollectionName))
                {
                    DocumentCollection col = 
                        FindDocumentDBCollectionByName(docDBCollectionName, client, database);

                    if (!string.IsNullOrEmpty(col.ResourceId))
                        collectionResourceId = col.ResourceId;
                }
            }
            return (databaseResourceId, collectionResourceId);
        }

        /// <summary>
        /// Trys to locate the DocumentDB Collection object from the DocumentClient and Database
        /// </summary>
        /// <param name="docDBCollectionName"></param>
        /// <param name="client"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        private static DocumentCollection FindDocumentDBCollectionByName(string docDBCollectionName, DocumentClient client, Database database)
        {
            return client.CreateDocumentCollectionQuery(database.SelfLink)
                                 .Where(c => c.Id == docDBCollectionName)
                                 .AsEnumerable()
                                 .FirstOrDefault();
        }

        /// <summary>
        /// Trys to locate the DocumentDB Database object from the DocumentClient 
        /// </summary>
        /// <param name="docDBName"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private static Database FindDocumentDBDatabaseByName(string docDBName, DocumentClient client)
        {
            return client.CreateDatabaseQuery().Where(d => d.Id == docDBName)
                       .AsEnumerable()
                      .FirstOrDefault();
        }

        #endregion

        /// <summary>
        /// Requests the metric definitions from an Azure resource. This can either be retrieved directly from the resource
        /// (leave provider parameter=null) or from microsoft.insights provider (set provider to "microsoft.insights").
        /// The resulting JSON will vary depending on the choice of the provider!
        /// 
        /// Request Url for metric definitions of resource:
        /// https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{rgname}/providers/Microsoft.DocumentDB/databaseAccounts/{docDBAccount}/metricDefinitions?api-version=2016-03-01
        /// 
        /// Request Url for metric definitions of resource via microsoft.insights:
        /// https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{rgname}/providers/Microsoft.DocumentDB/databaseAccounts/{docDBAccount}/providers/microsoft.insights/metricDefinitions?api-version=2018-01-01
        /// 
        /// Azure Reference Documentation
        /// =============================
        /// DocumentDB Database resourceUri:
        /// https://docs.microsoft.com/en-us/rest/api/cosmos-db-resource-provider/database/database_listmetricdefinitions
        /// 
        /// DocumentDB Collection resourceUri:
        /// https://docs.microsoft.com/en-us/rest/api/cosmos-db-resource-provider/collection/collection_listmetricdefinitions
        /// 
        /// </summary>
        /// <param name="resourceUri">Uri to the azure resource: /subscriptions/{subscriptionid}/resourceGroups/{resourceGroup}/providers/{ResourceProvider}/ResourcePath
        /// Example:
        ///     /subscriptions/23c22123-ab12-1a2b-ab1c-a2b2f1234567/resourceGroups/myRgName/providers/Microsoft.DocumentDB/databaseAccounts/myCosmosdbaccountName/
        /// </param>
        /// <param name="apiVersion">API-Version to use for Metric definitions</param>
        /// <returns></returns>
        public async Task<string> GetMetricDefinitionsAsJSONAsync(string resourceUri, string apiVersion=null)
        {
            StringBuilder requestUri = CreateBaseRequestUri(resourceUri, "metricDefinitions", apiVersion);
            HttpResponseMessage response = await this.HttpClient.GetAsync(requestUri.ToString());
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Function to request Metrics from either Azure Monitor or CosmosDB Resource Provider.
        /// The result is returned as JSON.
        /// </summary>
        /// <param name="resourceUri">Resource of which metric should be consumed</param>
        /// <param name="oDataQueryBuilderDelegate">Delegate to an inline lambda function to build a simple oDataQuery with a Builder class.</param>
        /// <param name="insightsQueryBuilderDelegate">Delegate to an inline lambda function to create the query parameters for an Azure Monitor Query with a Builder class</param>
        /// <param name="apiVersion">Alternative API-Version, if default is not sufficient</param>
        /// <returns>Metrics in JSON-Format</returns>
        public async Task<string> GetMetricAsJSONAsync(
            string resourceUri,
            Action<ODataQueryBuilder> oDataQueryBuilderDelegate,
            Action<InsightsQueryBuilder> insightsQueryBuilderDelegate = null,
            string apiVersion = null)
        {
            StringBuilder requestUri = CreateBaseRequestUri(resourceUri, "metrics", apiVersion);
            InsightsQueryBuilder insightsQueryBuilder = new InsightsQueryBuilder();
            ODataQueryBuilder oDataQueryBuilder = null;

            if (oDataQueryBuilderDelegate != null)
            {
                oDataQueryBuilder = new ODataQueryBuilder();
                oDataQueryBuilderDelegate(oDataQueryBuilder);
            }
            if (insightsQueryBuilderDelegate!=null)
            {
                insightsQueryBuilderDelegate(insightsQueryBuilder);
                insightsQueryBuilder.AddODataQuery(oDataQueryBuilder);
                requestUri.Append("&");
                requestUri.Append(insightsQueryBuilder.ToString());
            }
            else
            {
                if (oDataQueryBuilderDelegate != null)
                {
                    string oDataQuery = oDataQueryBuilder.ToString();
                    if (!string.IsNullOrEmpty(oDataQuery))
                    {
                        requestUri.Append("&$filter=");
                        requestUri.Append(oDataQuery);
                    }
                }
            }
            HttpResponseMessage response = await this.HttpClient.GetAsync(requestUri.ToString());
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.ReasonPhrase);
        }
    }
}
