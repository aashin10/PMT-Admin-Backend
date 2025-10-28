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

                case DuplicateEntryException duplicateEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = new
                    {
                        Status = statusCode,
                        Data = (object)null,
                        Message = duplicateEx.Message
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

                case Microsoft.EntityFrameworkCore.DbUpdateException dbUpdateEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    var errorMessage = "Database error occurred";

                    // Check for duplicate key violations
                    if (dbUpdateEx.InnerException?.Message?.ToLower().Contains("duplicate") == true ||
                        dbUpdateEx.InnerException?.Message?.ToLower().Contains("unique") == true)
                    {
                        var innerMsg = dbUpdateEx.InnerException.Message.ToLower();
                        if (innerMsg.Contains("email") || innerMsg.Contains("ix_users_email"))
                        {
                            errorMessage = "Email already exists";
                        }
                        else if (innerMsg.Contains("jira") || innerMsg.Contains("ix_users_jiraid"))
                        {
                            errorMessage = "Jira ID already exists";
                        }
                        else
                        {
                            errorMessage = "Duplicate entry detected";
                        }
                    }

                    apiResponse = new
                    {
                        Status = statusCode,
                        Data = (object)null,
                        Message = errorMessage
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
