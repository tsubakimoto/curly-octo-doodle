using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FunctionApp1;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var userId = req.Query["userId"].ToString();
        var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString")
            ?? throw new InvalidOperationException("SqlConnectionString is not set.");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // Intentionally vulnerable sample for CodeQL SQL injection detection.
        var sql = "SELECT * FROM Users WHERE Id = '" + userId + "'";
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        _logger.LogInformation("Executed sample query for userId: {UserId}", userId);
        return new OkObjectResult("Executed intentionally vulnerable query sample.");
    }
}