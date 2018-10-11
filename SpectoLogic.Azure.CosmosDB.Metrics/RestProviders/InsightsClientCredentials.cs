using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SpectoLogic.Azure.CosmosDB.Metrics.Extensions;
using System.Diagnostics;

namespace SpectoLogic.Azure.CosmosDB.Metrics.RestProviders
{
    /// <summary>
    /// Author:     Andreas Pollak 
    /// Copyright:  (c) by SpectoLogic
    /// </summary>
    public class InsightsClientCredentials : ServiceClientCredentials
    {
        private string _tenantId = null;
        private string _clientId = null;
        private string _clientSecret = null;
        private string _resource = "https://management.core.windows.net/";
        private string _apiVersion = null;

        public InsightsClientCredentials(string clientId, string clientSecret, string tenantId, string apiVersion = null)
        {
            _tenantId = tenantId;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _apiVersion = apiVersion;
        }

        private string AuthenticationToken { get; set; }
        public override void InitializeServiceClient<T>(ServiceClient<T> client)
        {
            var authenticationContext =
                new AuthenticationContext($"https://login.windows.net/{_tenantId}");
            var credential = new ClientCredential(clientId: _clientId, clientSecret: _clientSecret);

            var result = authenticationContext.AcquireTokenAsync(resource: _resource,
                clientCredential: credential).Result;

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }
            AuthenticationToken = result.AccessToken;
        }

        private Uri ReplaceApiVersion(Uri theRequestUri, string newApiVersion)
        {
            return theRequestUri.ReplaceQueryParameter("api-version", newApiVersion);
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (AuthenticationToken == null)
                throw new InvalidOperationException("Token Provider Cannot Be Null");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AuthenticationToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (_apiVersion != null)
                request.RequestUri = ReplaceApiVersion(request.RequestUri, _apiVersion);
#if DEBUG
            Debug.WriteLine(request.RequestUri);
#endif
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }

}
