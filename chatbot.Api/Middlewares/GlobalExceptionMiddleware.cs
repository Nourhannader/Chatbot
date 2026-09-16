namespace chatbot.Api.Middlewares
{
    public class GlobalExceptionMiddleware(RequestDelegate next,ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                await GlobalExceptionHandler.HandleExceptionAsync(context,ex);
            }
        }
    }
}
