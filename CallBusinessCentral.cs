using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace Demo_Managed_Identity;

public class CallBusinessCentral
{
    private readonly ILogger<CallBusinessCentral> _logger;
    private const string BCEnvironmentName = "directions-emea";
    private const string BCCompanyId = "b53163cc-f6bb-f111-85c5-7ced8db4483e";
    private const string BCBaseUrl = "https://api.businesscentral.dynamics.com/v2.0/{BCEnvironmentName}/api/v2.0/companies({BCCompanyId})";
    private static readonly HttpClient _httpClient = new HttpClient();

    public CallBusinessCentral(ILogger<CallBusinessCentral> logger)
    {
        _logger = logger;
    }

    [Function("CallBusinessCentral")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("Starting Business Central API call via Managed Identity.");

        try
        {
            // 1. Use DefaultAzureCredential (works locally via Azure CLI/VS, and in Azure via Managed Identity)
            var credential = new DefaultAzureCredential();

            // 2. SPECIFIC SCOPE FOR BUSINESS CENTRAL
            string[] scopes = new[] { "https://api.businesscentral.dynamics.com/.default" };
            var tokenContext = new TokenRequestContext(scopes);

            // 3. Obtain the access token
            AccessToken tokenResult = await credential.GetTokenAsync(tokenContext);
            string accessToken = tokenResult.Token;

            // 4. Prepare the HTTP call to Business Central
            // Replace this with your specific BC environment target (environment, company, endpoint)
            string bcUrl = BCBaseUrl.Replace("{BCEnvironmentName}", BCEnvironmentName).Replace("{BCCompanyId}", BCCompanyId) + "/customers";

            var request = new HttpRequestMessage(HttpMethod.Get, bcUrl);

            // 5. PUT THE TOKEN IN THE AUTHORIZATION HEADER
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // BC APIs expect JSON
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 6. Execute the call
            HttpResponseMessage bcResponse = await _httpClient.SendAsync(request);
            string responseContent = await bcResponse.Content.ReadAsStringAsync();

            if (bcResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully retrieved data from Business Central!");

                var response = new OkObjectResult(responseContent);
                return response;
            }
            else
            {
                _logger.LogError($"Business Central returned an error: {bcResponse.StatusCode} - {responseContent}");

                return new ContentResult
                {
                    Content = responseContent,
                    ContentType = "application/json",
                    StatusCode = (int)bcResponse.StatusCode
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}");
            return new ObjectResult($"Internal error: {ex.Message}")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}