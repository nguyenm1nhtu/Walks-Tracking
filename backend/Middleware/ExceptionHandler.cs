using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Walks.API.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandler(
            RequestDelegate next,
            ILogger<ExceptionHandler> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for request {Method} {Path}", context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response has already started. Exception middleware will rethrow.");
                    throw;
                }

                var (statusCode, title) = MapException(ex);

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = _environment.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
                    Instance = context.Request.Path
                };

                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                if (_environment.IsDevelopment())
                {
                    problemDetails.Extensions["exception"] = ex.GetType().Name;
                }

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        }

        private static (int StatusCode, string Title) MapException(Exception ex)
        {
            return ex switch
            {
                ArgumentException => ((int)HttpStatusCode.BadRequest, "Bad Request"),
                UnauthorizedAccessException => ((int)HttpStatusCode.Forbidden, "Forbidden"),
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Not Found"),
                FileNotFoundException => ((int)HttpStatusCode.NotFound, "Not Found"),
                _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error")
            };
        }
    }

    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandler>();
        }
    }
}
