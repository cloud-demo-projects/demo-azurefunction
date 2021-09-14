using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net.Http;

namespace Cbsp.Foundation.Network.Api.Functions
{
    public class AvailabilityCheck
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly string instrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");
        private readonly string webJobAuthenticationToken = Environment.GetEnvironmentVariable("WEBJOB_AUTHORIZATION");
        private const string EndpointAddress = "https://dc.services.visualstudio.com/v2/track";

        public AvailabilityCheck()
        {
            _telemetryClient = new TelemetryClient(
            new TelemetryConfiguration(instrumentationKey,
            new InMemoryChannel { EndpointAddress = EndpointAddress }));
        }

        [FunctionName("AvailabilityCheck")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Entering the availability check at: {DateTime.Now}");

            string testName = "AvailabilityCheck";
            string location = Environment.GetEnvironmentVariable("REGION_NAME");
            string operationId = Guid.NewGuid().ToString("N");
            string testUrl = Environment.GetEnvironmentVariable("TEST_URL");
            if (String.IsNullOrEmpty(testUrl))
            {
                testUrl = "https://www.bing.com/";
            }

            var availabilityTelemetry = new AvailabilityTelemetry
            {
                Id = operationId,
                Name = testName,
                RunLocation = location,
                Success = false
            };

            try
            {
                var httpClient = new HttpClient();
                var result = await httpClient.GetAsync(testUrl);
                log.LogInformation($"Result: {result.StatusCode}");
                availabilityTelemetry.Success = true;
            }
            catch (Exception ex)
            {
                availabilityTelemetry.Message = ex.Message;

                var exceptionTelemetry = new ExceptionTelemetry(ex);
                exceptionTelemetry.Context.Operation.Id = operationId;
                exceptionTelemetry.Properties.Add("TestName", testName);
                exceptionTelemetry.Properties.Add("TestLocation", location);
                _telemetryClient.TrackException(exceptionTelemetry);
            }
            finally
            {
                _telemetryClient.TrackAvailability(availabilityTelemetry);
                _telemetryClient.Flush();
            }
        }
    }
}