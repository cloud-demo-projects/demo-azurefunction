using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using requests;
using System.Text.RegularExpressions;
using HttpTriggerCSharpSample.Validators;
using System;
using Services;

namespace Functions
{
    public class HttpTriggerCSharpSample
    {
        private readonly HttpClient _client;
        private readonly IMyService _service;
        private readonly IAzManagementApiService _azManagementApiService;
        private readonly RequestValidator _requestValidator;

        public HttpTriggerCSharpSample(HttpClient httpClient, IMyService service, IAzManagementApiService azManagementApiService, RequestValidator _requestValidator)
        {
            this._client = httpClient;
            this._service = service;
            this._azManagementApiService = azManagementApiService;
            this._requestValidator = _requestValidator;
        }

        [FunctionName("HttpTriggerCSharpSample")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            // Think User Vs App Authentication & Authorization
            // Authentication -Azure AD Authentication Enforced through Function
            // Authorization- Think providing delegated permissions to app registration/spn towards Microsoft Graph(User.Read) & Storage(user_impersonated)
            // Think of utilizing Azure AD Groups

            // Parse & Deserialize the Incoming Http Request
            string requestBody2 = Regex.Replace(await new StreamReader(req.Body).ReadToEndAsync(), @"\t|\n|\r", "");
            var incomingRequest = JsonConvert.DeserializeObject<TriggerRequest<TriggerPipelineRequestBody>>(requestBody2);
            log.LogInformation($"{incomingRequest.TriggerData.Environment.SubscriptionName}");

            // Validate Request through Fluent Validator
            var validateResult = _requestValidator.Validate(incomingRequest);
            if (!validateResult.IsValid)
            {
                log.LogError("Invalid Input Supplied", validateResult.Errors);
                return new BadRequestResult();
            }

            // Sample Service Calls using Httpclient
            var response = await _client.GetAsync("https://microsoft.com");
            log.LogInformation("Request for item with status code={response.StatusCode}.", response.StatusCode); //Traces in Application Insights
            var message = _service.WriteMessage();
            log.LogInformation("message received-", message);


            // Sample ARM REST API Call through Function MSI(System assigned)
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            HttpResponseMessage Targetresp = await _azManagementApiService.GetSubscriptionDetails(requestBody, log);
            log.LogInformation("Targetresp for item with code={Targetresp.StatusCode}.", Targetresp.StatusCode);
            log.LogInformation("Targetresp for item with result={Targetresp.Content.ReadAsStringAsync().Result}.", Targetresp.Content.ReadAsStringAsync().Result);
            return Targetresp != null
                ? (ActionResult)new OkObjectResult($"Subscription Details- {Targetresp.Content.ReadAsStringAsync().Result}")
                : new BadRequestObjectResult("SUbscription details could not be fetched due to bad request.");


            // Retrieve KeyVault Secrets through MSI
            string secretValue = Environment.GetEnvironmentVariable("mysecret");
            log.LogInformation($"KV value - {secretValue}.");


            // Sample Azure DevOps REST API call through PAT retrieved from KeyVault
            // TODO

            // Sample Azure Resource Graph call through Function MSI
            // TODO

            // Sample Microsoft Graph API call through Function MSI
            // TODO

            // Sample Cosmos DB Interaction
            // TODO

            // Sample Output Binding
            // TODO

        }

    }
}
