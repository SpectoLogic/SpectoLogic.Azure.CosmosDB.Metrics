using Microsoft.Rest;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders
{
    /// <summary>
    /// HttpClientHandle which preprocesses the request through an instance of ServiceClientCredentials
    /// (aka Authentication)
    /// Author:    Andreas Pollak 
    /// Copyright: (c) by SpectoLogic
    /// </summary>
    public class AzureBaseInsightsHttpHandler : HttpClientHandler
    {
        ServiceClientCredentials _credentials;
        public AzureBaseInsightsHttpHandler(ServiceClientCredentials credentials)
        {
            this._credentials = credentials;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await _credentials.ProcessHttpRequestAsync(request, cancellationToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
