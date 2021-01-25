using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Exchange.API.Common;
using Exchange.API.Services.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Exchange.API.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                int statusCode = StatusCodes.Status400BadRequest;

                string title = string.IsNullOrEmpty(ex.Title) ? "An error has occurred." : ex.Title;

                var errorJson = "";
                if (ex.Errors != null && ex.Errors.Count > 0)
                {
                    errorJson = " " + JsonSerializer.Serialize(ex.Errors);
                }

                _logger.LogError(ex, "{Message} {StatusCode}", ex.Message + errorJson, statusCode);

                await SendError(context, title, ex.Message, statusCode, ex.Errors);
            }
            catch (NotFoundHttpException ex)
            {
                int statusCode = StatusCodes.Status404NotFound;
                string title = string.IsNullOrEmpty(ex.Title) ? "An error has occurred." : ex.Title;

                _logger.LogError(ex, "{Message} {StatusCode}", ex.Message, statusCode);

                await SendError(context, title, ex.Message, statusCode);
            }
            catch (Exception ex)
            {
                int statusCode = StatusCodes.Status500InternalServerError;

                _logger.LogError(ex, "{Message} {StatusCode}", ex.Message, statusCode);

                await SendError(context, "An error has occurred.", ex.Message, statusCode);
            }
        }

        private async Task SendError(HttpContext context, string title, string message, int statusCode, IDictionary<string, string[]> errors = null)
        {
            if (context.Response.HasStarted) return;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string jsonMessage;

            if (statusCode == 400)
            {
                if (errors == null)
                {
                    var errorDetails = ApiErrorResponse.GetBadRequestMessage(message, context.Request.Path);
                    jsonMessage = JsonSerializer.Serialize(errorDetails, options);
                }
                else
                {
                    var errorDetails = ApiErrorResponse.GetBadRequestMessage(title, message, statusCode, context.Request.Path, errors);
                    jsonMessage = JsonSerializer.Serialize(errorDetails, options);
                }

            }
            else
            {
                var errorDetails = ApiErrorResponse.GetMessage(title, message, statusCode, context.Request.Path);
                jsonMessage = JsonSerializer.Serialize(errorDetails, options);
            }

            await context.Response.WriteAsync(jsonMessage);
        }
    }
}
