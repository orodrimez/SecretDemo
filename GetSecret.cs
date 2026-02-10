using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SecretDemo;

public class GetSecret
{
    private readonly ILogger<GetSecret> _logger;

    public GetSecret(ILogger<GetSecret> logger)
    {
        _logger = logger;
    }

    [Function("GetSecret")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        var keyVaultUrl = Environment.GetEnvironmentVariable("KeyVaultUrl");

        var client = new SecretClient(
            new Uri(keyVaultUrl),
            new DefaultAzureCredential()
        );

        var secret = await client.GetSecretAsync("DemoSecret");

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync(secret.Value.Value);

        return response;
    }
}
