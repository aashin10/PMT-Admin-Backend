using PmtAdmin.Application.CustomException;
using System.Net;
using System.Text.Json;

namespace PmtAdmin.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            object apiResponse;
            int statusCode;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = new
                    {
                        Status = statusCode,
                        Data = (object)null,
                        Message = validationEx.Message,
                        Errors = validationEx.Errors
                    };
                    break;

                case NotFoundException notFoundEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    apiResponse = new
                    {
                        Status = statusCode,
                        Data = (object)null,
                        Message = notFoundEx.Message
                    };
                    break;

                case UnauthorizedAccessException unauthorizedEx:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    apiResponse = new
                    {
                        Status = statusCode,
                        Data = (object)null,
                        Message = unauthorizedEx.Message
                    };
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    apiResponse = new
                    {
                        Status = statusCode,
                        Data = (object)null,
                        Message = "An unexpected error occurred.",
                        Details = exception.Message
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;
            var json = JsonSerializer.Serialize(apiResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
