using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using Configurations;

namespace Services
{
    public class AzManagementApiService : IAzManagementApiService
    {
        private readonly HttpClient _client;
        private readonly ILogger _log;
        private readonly IOptions<AzAdOptions> _azAdOptions;

        public AzManagementApiService(HttpClient httpClient, IOptions<AzAdOptions> azAdOptions, ILogger log)
        {
            this._client    = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            //this._log       = log ?? throw new ArgumentNullException(nameof(log));
            this._azAdOptions = azAdOptions ?? throw new ArgumentNullException(nameof(azAdOptions));
        }

        public async Task<HttpResponseMessage> GetSubscriptionDetails(string requestBody, ILogger log)
        {
            log.LogInformation("Calling GetSubscriptionDetails");        
            HttpResponseMessage response = null;
            string projectResource = "https://management.azure.com/";

            try{
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(resource:projectResource, tenantId:$"{_azAdOptions.Value.TenantId}");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                    response = httpClient.GetAsync("https://management.azure.com/subscriptions/bf0f6779-86d2-467e-8226-10f92a8ad378?api-version=2020-01-01").Result;
                }
                log.LogInformation("response for item with key={response}.", response);
            }
            catch(Exception ex){
                log.LogError("Exception caught while calling ARM REST API" + ex);
            }
            return response;
        }
    }
}
