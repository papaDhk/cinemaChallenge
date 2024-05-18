using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Services;
using Microsoft.AspNetCore.Http;

namespace ApiApplication.Middleware
{
    public class CustomExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
              await HandleException(context, ex, 404, ex.Code, ex.Message);
            }
            catch (CustomException ex)
            {
                await HandleException(context, ex, 400, ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex, 500, "InternalError", "An internal error occured while processing your request, please contact the support");
            }
        }

        private static async Task HandleException(HttpContext context, Exception ex, int statusCode, string errorCode, string errorMessage)
        {
            LogException(ex);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new{ErrorCode = errorCode, ErrorMessage = errorMessage }));
        }
        
        private static void LogException(Exception ex)
        {
            // Implement your custom logging mechanism here
            // Log the exception details, including message, stack trace, and relevant information
        } 
    }
}