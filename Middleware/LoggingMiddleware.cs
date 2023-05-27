using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly string _logFilePath = "log.txt"; // Path untuk menyimpan file log

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Membaca body request
        var requestBodyStream = new MemoryStream();
        var originalRequestBody = context.Request.Body;
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
        context.Request.Body = originalRequestBody;

        // Mencatat waktu dan endpoint
        var requestTime = DateTime.UtcNow;
        var endpoint = $"{context.Request.Method} {context.Request.Path}";

        // Menjalankan request dan menangkap response
        var originalResponseBody = context.Response.Body;
        using (var responseBodyStream = new MemoryStream())
        {
            context.Response.Body = responseBodyStream;

            await _next(context);

            // Membaca body response
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // Mencatat log
            var logObject = new
            {
                Time = requestTime,
                Endpoint = endpoint,
                RequestBody = requestBodyText,
                ResponseBody = responseBodyText
            };

            var logMessage = JsonConvert.SerializeObject(logObject, Newtonsoft.Json.Formatting.Indented);

            _logger.LogInformation(logMessage);

            // Menyimpan log ke file
            try
            {
                using (var writer = File.AppendText(_logFilePath))
                {
                    await writer.WriteLineAsync(logMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to write log to file: {ex.Message}");
            }

            // Mengembalikan response ke aslinya
            await responseBodyStream.CopyToAsync(originalResponseBody);
        }
    }
}