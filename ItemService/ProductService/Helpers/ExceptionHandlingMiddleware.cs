using Entities.Dtos;
using Services;
using System.Net;
using System.Text.Json;

namespace AProductService.Helpers
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }

            catch (ConflictException c)
            {
                await context.Response.WriteAsync(HandleException(409, "Conflict", c.Message, context));
            }

            catch (ForbiddenException f)
            {
                await context.Response.WriteAsync(HandleException(403, "Forbidden", f.Message, context));
            }

            catch (NotFoundException n)
            {
                await context.Response.WriteAsync(HandleException(404, "Not found", n.Message, context));
            }
        }

        /// <summary>
        /// Handles all the catch block operations 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="description"></param>
        /// <param name="context"></param>
        /// <returns></returns>

        public string HandleException(int statusCode, string message, string description, HttpContext context)
        {
            // Uses ErrorResponseDto to set status code, message and description for the condition

            ErrorResponseDto errorResponse = new ErrorResponseDto() { StatusCode = statusCode, Message = message, Description = description };

            // Create an instance of the JsonSerializerOptions class

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            };

            // Serialize the object to a JSON string

            string json = JsonSerializer.Serialize(errorResponse, options);

            // Sets the content type/status code

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            // Write the serialized JSON string to the response body and return it to the client

            return json;
        }
    }
}

