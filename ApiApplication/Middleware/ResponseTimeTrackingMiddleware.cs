using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Middleware
{
    public class ResponseTimeTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseTimeTrackingMiddleware> _logger;

        public ResponseTimeTrackingMiddleware(RequestDelegate next, ILogger<ResponseTimeTrackingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            var route = context.Request.Path.Value;
            
            _logger.LogInformation($"Request to {route} took {stopwatch.ElapsedMilliseconds} milliseconds");
        }
    }
}