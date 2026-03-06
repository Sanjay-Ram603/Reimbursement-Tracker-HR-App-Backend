using System.Net;
using System.Text.Json;

namespace ReimbursementTrackerApp.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = ex.InnerException?.Message ?? ex.Message

                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
