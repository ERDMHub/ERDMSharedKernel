using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace ERDM.Shared.Kernel.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path} " +
                              $"took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
