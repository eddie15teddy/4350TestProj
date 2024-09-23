using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using MySqlConnector;
using Microsoft.Extensions.Options;
using Azure.Core;
using MySqlConnector.Authentication;
using Microsoft.Azure.Services.AppAuthentication;

namespace MySpace
{
    public class TestFunction
    {
        private readonly ILogger<TestFunction> _logger;

        public TestFunction(ILogger<TestFunction> logger)
        {
            _logger = logger;
        }

        [Function("TestFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string responseString = "Add num1 and num2 querry params.<br>(example: ?num1=1&num2=8)";
            string ?num1 = req.Query["num1"];
            string ?num2 = req.Query["num2"];

            if (!string.IsNullOrEmpty(num1) && !string.IsNullOrEmpty(num2)) 
            {
                responseString = $"{num1} + {num2} = {num1 + num2}";
            }

            var tokenCredential = new AzureServiceTokenProvider();

            var token = await tokenCredential.GetAccessTokenAsync("https://ossrdbms-aad.database.windows.net");

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "4350test.mysql.database.azure.com",
                Port = 3306,
                Database = "testdb",
                UserID = "testfunctionappuser@4350test",
                Password = token,  
                SslMode = MySqlSslMode.Required              
            };
            _logger.LogInformation(builder.ConnectionString);
            

            using var conn = new MySqlConnection(builder.ConnectionString);
            

            _logger.LogInformation("Opening connection");
            conn.Open();
            _logger.LogInformation("I think its working");
            

            return new ContentResult
            {
                Content = $"<html><body><h1>I am just a dummy endpoint</h1><p>{responseString}<p></body></html>",
                ContentType = "text/html",
                StatusCode = 200
            };
        }
    }
}
