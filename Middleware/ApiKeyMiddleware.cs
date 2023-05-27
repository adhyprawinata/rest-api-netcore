using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace rest_api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-Key";
        private const string ValidApiKey = "CIMBNIAGA"; // Ganti dengan API key yang valid

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!IsValidApiKey(context.Request.Headers[ApiKeyHeaderName]))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //await context.Response.WriteAsync("Invalid API key");
                //return;
                var response = new
                {
                    status = "401",
                    message = "Unauthorized",
                    data = ""
                };

                await WriteJsonResponse(context.Response, response);
                return;
            }

            await _next.Invoke(context);
        }

        private bool IsValidApiKey(string apiKey)
        {
            return apiKey == ValidApiKey;
        }

        private static async Task WriteJsonResponse(HttpResponse response, object data)
        {
            response.ContentType = "application/json";
            await response.WriteAsync(JsonSerializer.Serialize(data));
        }
    }
}