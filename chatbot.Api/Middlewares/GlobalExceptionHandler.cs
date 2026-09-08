using Microsoft.AspNetCore.Diagnostics;

namespace chatbot.Api.Middlewares
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception,"Unhandled exception");

            httpContext.Response.StatusCode = 500;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                },
                cancellationToken);

            return true;
        }
    }
}
