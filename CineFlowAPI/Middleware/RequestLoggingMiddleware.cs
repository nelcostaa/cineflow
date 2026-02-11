using System.Diagnostics;
using System.Text;

namespace Cineflow.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];

        await LogRequest(context, requestId);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            stopwatch.Stop();

            await LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);

            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ [{RequestId}] ERRO: {Message}", requestId, ex.Message);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context, string requestId)
    {
        var request = context.Request;

        var methodColor = request.Method switch
        {
            "GET" => "🟢",
            "POST" => "🟡",
            "PUT" => "🔵",
            "DELETE" => "🔴",
            _ => "⚪"
        };

        var logBuilder = new StringBuilder();
        logBuilder.AppendLine($"\n{'=',-80}");
        logBuilder.AppendLine($"🔹 [{requestId}] {methodColor} {request.Method} {request.Path}{request.QueryString}");
        logBuilder.AppendLine($"📍 Endpoint: {request.Scheme}://{request.Host}{request.Path}");

        // Query Parameters
        if (request.QueryString.HasValue)
        {
            logBuilder.AppendLine($"🔍 Query Params:");
            foreach (var param in request.Query)
            {
                logBuilder.AppendLine($"   • {param.Key} = {param.Value}");
            }
        }

        // Headers importantes
        logBuilder.AppendLine($"📋 Headers:");
        logBuilder.AppendLine($"   • Content-Type: {request.ContentType ?? "N/A"}");
        logBuilder.AppendLine($"   • User-Agent: {request.Headers["User-Agent"].FirstOrDefault() ?? "N/A"}");

        // Body da requisição (para POST/PUT)
        if (request.ContentLength > 0 && (request.Method == "POST" || request.Method == "PUT"))
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;

            logBuilder.AppendLine($"📦 Request Body:");
            logBuilder.AppendLine($"   {FormatJson(bodyAsText)}");
        }

        _logger.LogInformation(logBuilder.ToString());
    }

    private async Task LogResponse(HttpContext context, string requestId, long elapsedMilliseconds)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var statusIcon = context.Response.StatusCode switch
        {
            >= 200 and < 300 => "✅",
            >= 300 and < 400 => "➡️",
            >= 400 and < 500 => "⚠️",
            >= 500 => "❌",
            _ => "ℹ️"
        };

        var logBuilder = new StringBuilder();
        logBuilder.AppendLine($"🔸 [{requestId}] {statusIcon} Response: {context.Response.StatusCode}");
        logBuilder.AppendLine($"⏱️  Tempo: {elapsedMilliseconds}ms");

        // Response body (limitar tamanho para não poluir muito)
        if (!string.IsNullOrEmpty(bodyText) && bodyText.Length > 0)
        {
            var truncatedBody = bodyText.Length > 2000
                ? bodyText.Substring(0, 2000) + "... (truncado)"
                : bodyText;

            logBuilder.AppendLine($"📤 Response Body:");
            logBuilder.AppendLine($"   {FormatJson(truncatedBody)}");
        }

        logBuilder.AppendLine($"{'=',-80}\n");

        _logger.LogInformation(logBuilder.ToString());
    }

    private string FormatJson(string json)
    {
        try
        {
            var obj = System.Text.Json.JsonSerializer.Deserialize<object>(json);
            return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            }).Replace("\n", "\n   ");
        }
        catch
        {
            return json;
        }
    }
}
