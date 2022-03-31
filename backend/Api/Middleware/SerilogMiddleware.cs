using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace YouFoos.Api.Middleware
{
    /// <summary>
    /// This middleware is used to log better information about network requests made using serilog.
    /// </summary>
    /// <remarks>
    /// Inspired by the article found at blog.getseq.net/smart-logging-middleware-for-asp-net-core/
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class SerilogMiddleware
    {
        private const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private static readonly ILogger Log = Serilog.Log.Logger;

        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerilogMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Standard middleware invoke method which runs the middleware.
        /// </summary>
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next(httpContext);
                stopwatch.Stop();

                // HTTP 500 responses are errors, but 200, 300, and 400 responses are normal behavior
                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode >= 500 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : Log;
                log.Write(level, 
                          MessageTemplate, 
                          httpContext.Request.Method, 
                          httpContext.Request.Path, 
                          statusCode, 
                          stopwatch.Elapsed.TotalMilliseconds);
            }
            // This try block never catches anything because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, stopwatch, ex)) { }
        }

        private static bool LogException(HttpContext httpContext, Stopwatch stopwatch, Exception ex)
        {
            stopwatch.Stop();

            LogForErrorContext(httpContext)
                .Error(ex, 
                       MessageTemplate, 
                       httpContext.Request.Method, 
                       httpContext.Request.Path, 
                       500,
                       stopwatch.Elapsed.TotalMilliseconds);

            return false;
        }

        private static ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", 
                            request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                            destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            return result;
        }
    }
}
