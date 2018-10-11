using Microsoft.Rest;
using System.Text;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders
{
    /// <summary>
    /// Class to access the metrics and metric definitions via Azure Monitor REST API.
    /// Author:     Andreas Pollak 
    /// Copyright:  (c) by SpectoLogic
    /// </summary>
    public class AzureMonitorInsightsProvider : AzureBaseInsightsProvider
    {
        public AzureMonitorInsightsProvider(ServiceClientCredentials credentials)
            : base(credentials)
        {
        }

        /// <summary>
        /// Create RequestUri for Azure Monitor 
        /// </summary>
        /// <param name="resourceUri"></param>
        /// <param name="action"></param>
        /// <param name="apiVersion"></param>
        /// <returns></returns>
        protected override StringBuilder CreateBaseRequestUri(string resourceUri, string action, string apiVersion = null)
        {
            if (string.IsNullOrEmpty(apiVersion))
                apiVersion = "2018-01-01"; // upgraded to use a newer version than "2017-05-01-preview" which is used by the portal;
            StringBuilder requestUri = base.CreateResourceRequestUri(resourceUri);
            requestUri.Append($"providers/microsoft.insights/");
            requestUri.Append($"{action}?api-version={apiVersion}");
            return requestUri;
        }
    }
}
