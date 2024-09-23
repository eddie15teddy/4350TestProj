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
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string responseString = "Add num1 and num2 querry params.<br>(example: ?num1=1&num2=8)";
            string ?num1 = req.Query["num1"];
            string ?num2 = req.Query["num2"];

            if (!string.IsNullOrEmpty(num1) && !string.IsNullOrEmpty(num2)) 
            {
                responseString = $"{num1} + {num2} = {num1 + num2}";
            }

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "4350test.mysql.database.azure.com",
                Port = 3306,
                Database = "testdb",
                UserID = "eddie15teddy",
                Password = "1234567-nine",  
            };
            _logger.LogInformation(builder.ConnectionString);
            
            using var conn = new MySqlConnection(builder.ConnectionString);
            
            _logger.LogInformation("Opening connection");
            conn.Open();
            _logger.LogInformation("I think its working");

            using var command = conn.CreateCommand();
            command.CommandText = $"insert into access (access_time) value ('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');";
            command.ExecuteNonQuery();

            command.CommandText = "select count(*) from access;";
            var rowCount = command.ExecuteScalar();

            return new ContentResult
            {
                Content = $"<html><body><h1>I am just a dummy endpoint</h1><p>{responseString}</p><p>Number of Visits: {rowCount}</p></body></html>",
                ContentType = "text/html",
                StatusCode = 200
            };
        }
    }
}
