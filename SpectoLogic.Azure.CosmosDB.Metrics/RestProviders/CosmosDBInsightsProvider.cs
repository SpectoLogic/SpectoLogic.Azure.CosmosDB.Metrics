using Microsoft.Rest;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders
{
    /// <summary>
    /// Class to access the metrics and metric definitions via Azure CosmosDB Resource Provider.
    /// Author:     Andreas Pollak 
    /// Copyright:  (c) by SpectoLogic
    /// </summary>
    public class CosmosDBInsightsProvider : AzureBaseInsightsProvider
    {
        public CosmosDBInsightsProvider(ServiceClientCredentials credentials)
            : base(credentials)
        {
        }

        /// <summary>
        /// Create RequestUri for CosmosDB Resource Provider
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <param name="action"></param>
        /// <param name="apiVersion"></param>
        /// <returns></returns>
        protected override StringBuilder CreateBaseRequestUri(string resourceUri, string action, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(apiVersion))
                apiVersion = "2016-03-31";

            StringBuilder requestUri = CreateResourceRequestUri(resourceUri);
            requestUri.Append($"{action}?api-version={apiVersion}");
            return requestUri;
        }
    }
}
