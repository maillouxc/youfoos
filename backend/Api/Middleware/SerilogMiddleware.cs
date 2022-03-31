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
    /// This middleware is used to log information about all API requests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SerilogMiddleware
    {
        private const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerilogMiddleware(ILogger logger, RequestDelegate next)
        {
            _logger = logger;
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

                // HTTP 500 responses are errors, but 100, 200, 300, and 400 responses are normal behavior.
                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode >= 500 ? LogEventLevel.Error : LogEventLevel.Information;

                var logger = level == LogEventLevel.Error ? LogForErrorContext(httpContext, _logger) : _logger;
                
                logger.Write(
                    level, 
                    MessageTemplate, 
                    httpContext.Request.Method, 
                    httpContext.Request.Path, 
                    statusCode, 
                    stopwatch.Elapsed.TotalMilliseconds
                );
            }
            // This try block never catches anything because `LogException()` returns false.
            // It's just a useful way to log exceptions without unwinding the call stack.
            catch (Exception ex) when (LogException(httpContext, stopwatch, ex, _logger)) { }
        }

        private static bool LogException(HttpContext httpContext, Stopwatch stopwatch, Exception ex, ILogger logger)
        {
            stopwatch.Stop();

            LogForErrorContext(httpContext, logger).Error(
                ex, 
                MessageTemplate, 
                httpContext.Request.Method, 
                httpContext.Request.Path, 
                500,
                stopwatch.Elapsed.TotalMilliseconds
            );

            return false;
        }

        private static ILogger LogForErrorContext(HttpContext httpContext, ILogger logger)
        {
            var request = httpContext.Request;

            var result = logger
                .ForContext("RequestHeaders", 
                            request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                            destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            return logger;
        }
    }
}
