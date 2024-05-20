using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Services;
using Microsoft.AspNetCore.Http;

namespace ApiApplication.Middlewares
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
              await HandleException(context, ex, (int)HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
            catch (ShowtimeCreationException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
            catch (AuditoriumNotAvailableException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.Conflict, ex.Code, ex.Message);
            }
            catch (TicketAlreadyPaidException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.Conflict, ex.Code, ex.Message);
            }
            catch (SeatsReservationExpiredException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.Conflict, ex.Code, ex.Message);
            }
            catch (NotEnoughSeatsAvailableException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.Conflict, ex.Code, ex.Message);
            }
            catch (MoviesServiceNotAvailableException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.ServiceUnavailable, ex.Code, ex.Message);
            }
            catch (CustomException ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex, (int)HttpStatusCode.InternalServerError, "InternalError", "An internal error occured while processing your request, please contact the support");
            }
        }

        private static async Task HandleException(HttpContext context, Exception ex, int statusCode, string errorCode, string errorMessage)
        {
            LogException(ex);
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new Error { ErrorCode = errorCode, ErrorMessage = errorMessage }));
        }
        
        private static void LogException(Exception ex)
        {
            // Implement your custom logging mechanism here
            // Log the exception details, including message, stack trace, and relevant information
        } 
    }
}