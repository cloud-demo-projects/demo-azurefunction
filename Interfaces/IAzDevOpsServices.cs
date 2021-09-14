using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;


namespace Services
{
    public interface IAzDevOpsServices
    {
        Task<HttpResponseMessage> GetProjects(string requestBody, ILogger log);
    }
}