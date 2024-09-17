using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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

            string responseString = "I am just a dummy endpoint for COMP 4350. Add num1 and num2 querry params.<br>(example: ?num1=1&num2=8)";
            string ?num1 = req.Query["num1"];
            string ?num2 = req.Query["num2"];

            if (!string.IsNullOrEmpty(num1) && !string.IsNullOrEmpty(num2)) 
            {
                responseString = $"{num1} + {num2} = {num1 + num2}";
            }


            return new ContentResult
            {
                Content = $"<html><body><h1>{responseString}</h1></body></html>",
                ContentType = "text/html",
                StatusCode = 200
            };
        }
    }
}
