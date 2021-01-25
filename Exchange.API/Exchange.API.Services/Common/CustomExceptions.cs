using System;
using System.Collections.Generic;

namespace Exchange.API.Services.Common
{
    public class NotFoundHttpException : Exception
    {
        public string Title;
        public NotFoundHttpException(string message) : base(message) { }
        public NotFoundHttpException(string title, string message) : base(message)
        {
            Title = title;
        }
    }

    public class BadRequestException : Exception
    {
        public string Title;
        public IDictionary<string, string[]> Errors;
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string title, IDictionary<string, string[]> errors, string message) : base(message)
        {
            Title = title;
            Errors = errors;
        }
    }
}
