using System;
using System.IO;
using System.Threading.Tasks;
using Exchange.API.Middleware;
using Exchange.API.Services.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Exchange.API.Test.UnitTest.Middleware
{
    public class GlobalExceptionMiddlewareTests
    {
        public interface IRequestDelegateMock
        {
            Task Next(HttpContext context);
        }

        private readonly HttpContext _context;
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
        private readonly Mock<IRequestDelegateMock> _mockRequestDelegate;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _context = new DefaultHttpContext();
            _context.Response.Body = new MemoryStream();
            _mockRequestDelegate = new Mock<IRequestDelegateMock>();
            _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _middleware = new GlobalExceptionMiddleware(_mockRequestDelegate.Object.Next, _mockLogger.Object);
        }

        [Fact]
        public async Task NextDelegate_Is_Invoked_ExactlyOnce()
        {
            // Arrange
            _mockRequestDelegate.Setup(x => x.Next(_context));

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            _mockRequestDelegate.Verify(x => x.Next(_context), Times.Once());
        }

        [Fact]
        public async Task NextDelegate_With_No_Exception_Creates_200Response()
        {
            var statusCode = StatusCodes.Status200OK;
            await NextDelegate_With_No_Exception_Creates_HttpResponse(statusCode);
        }

        [Fact]
        public async Task NextDelegate_With_No_Exception_Creates_401Response()
        {
            var statusCode = StatusCodes.Status401Unauthorized;
            await NextDelegate_With_No_Exception_Creates_HttpResponse(statusCode);
        }

        [Fact]
        public async Task NextDelegate_With_No_Exception_Creates_404Response()
        {
            var statusCode = StatusCodes.Status404NotFound;
            await NextDelegate_With_No_Exception_Creates_HttpResponse(statusCode);
        }

        [Fact]
        public async Task NextDelegate_With_No_Exception_Creates_500Response()
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            await NextDelegate_With_No_Exception_Creates_HttpResponse(statusCode);
        }

        [Fact]
        public async Task NextDelegate_With_NotFoundHttpException_Creates_404Response()
        {
            var exception = new NotFoundHttpException("Not found.");
            var statusCode = StatusCodes.Status404NotFound;

            await NextDelegate_With_Exception_Creates_ErrorResponse(exception, statusCode);
        }

        [Fact]
        public async Task NextDelegate_With_Generic_Exception_Creates_500Response()
        {
            var exception = new Exception("Server error");
            var statusCode = StatusCodes.Status500InternalServerError;

            await NextDelegate_With_Exception_Creates_ErrorResponse(exception, statusCode);
        }

        private async Task NextDelegate_With_No_Exception_Creates_HttpResponse(int statusCode)
        {
            // Arrange
            _context.Response.StatusCode = statusCode;

            _mockRequestDelegate.Setup(x => x.Next(_context))
                .Returns(Task.FromResult(0))
                .Verifiable();

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            Assert.Equal(statusCode, _context.Response.StatusCode);
            _mockRequestDelegate.Verify();
        }

        private async Task NextDelegate_With_Exception_Creates_ErrorResponse(Exception exception, int statusCode)
        {
            // Arrange
            _mockLogger.Setup(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(),
                    It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
                .Verifiable();

            _mockRequestDelegate.Setup(x => x.Next(_context))
                .Throws(exception)
                .Verifiable();

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            Assert.Equal(statusCode, _context.Response.StatusCode);
            var errorDetails = GetHttpResponseError(_context);
            Assert.NotNull(errorDetails);
            Assert.Equal(statusCode, errorDetails.Status);

            _mockRequestDelegate.Verify();
        }

        private ProblemDetails GetHttpResponseError(HttpContext context)
        {
            _context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(_context.Response.Body);
            var responseText = reader.ReadToEnd();

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => args.ErrorContext.Handled = true
            };

            return JsonConvert.DeserializeObject<ProblemDetails>(responseText, settings);

        }

    }
}
