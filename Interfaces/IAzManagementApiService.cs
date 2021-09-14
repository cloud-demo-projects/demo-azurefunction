using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;


namespace Services
{
    public interface IAzManagementApiService
    {
        Task<HttpResponseMessage> GetSubscriptionDetails(string requestBody, ILogger log);
    }
}