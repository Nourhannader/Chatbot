using chatbot.Core.Common;
using chatbot.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace chatbot.Api.Middlewares
{
    public static class GlobalExceptionHandler
    {
        public static async Task HandleExceptionAsync(HttpContext context,Exception exception)
        {
            context.Response.ContentType ="application/json";

            var response =
                new ApiResponse<object>
                {
                    Success = false
                };
            switch (exception)
            {
                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    response.Message =exception.Message;
                    break;
                case ForbiddenException:
                    context.Response.StatusCode =StatusCodes.Status403Forbidden;

                    response.Message =exception.Message;
                    break;
                case UnauthorizedException:
                    context.Response.StatusCode =StatusCodes.Status401Unauthorized;

                    response.Message =exception.Message;
                    break;
                case ConflictException:
                    context.Response.StatusCode =StatusCodes.Status409Conflict;

                    response.Message =exception.Message;
                    break;
                case ValidationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    response.Message = exception.Message;
                    break;
                case RefreshTokenReuseException:
                    context.Response.StatusCode = StatusCodes.Status511NetworkAuthenticationRequired;

                    response.Message = exception.Message;
                    break;
                case BadRequestException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    response.Message = exception.Message;
                    break;
                default:

                    context.Response.StatusCode =StatusCodes.Status500InternalServerError;

                    response.Message ="An unexpected error occurred.";
                    break;
            }
            await context.Response.WriteAsJsonAsync(response);

        }
    }
}
