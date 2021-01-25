using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.API.Common
{
    public class ApiErrorResponse
    {
        public static ProblemDetails GetMessage(string title, string message, int httpStatusCode, string path)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = message,
                Status = httpStatusCode,
                Instance = path,
                Type = ""
            };
        }

        public static ProblemDetails GetBadRequestMessage(string message, string path)
        {
            return GetMessage($"Bad Request",
              $"Bad Request: {message}",
              StatusCodes.Status400BadRequest, path);
        }

        public static ValidationProblemDetails GetBadRequestMessage(string title, string message, int httpStatusCode,
            string path, IDictionary<string, string[]> errors)
        {
            return new ValidationProblemDetails(errors)
            {
                Title = title,
                Detail = message,
                Status = httpStatusCode,
                Instance = path,
                Type = ""
            };
        }
    }
}
