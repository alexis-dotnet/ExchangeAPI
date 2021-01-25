using System;
using System.Threading.Tasks;
using Exchange.API.Controllers;
using Exchange.API.Dto.Exchange;
using Exchange.API.Services.Common;
using Exchange.API.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Exchange.API.Test.UnitTest.Controllers
{
    public class ExchangeControllerTests
    {
        private readonly ExchangeController _controller;
        private readonly Mock<IRateService> _mockRateService;
        private readonly Mock<IOptions<ApiOptions>> _mockOptions;

        public ExchangeControllerTests()
        {
            var mockLogger = new Mock<ILogger<ExchangeController>>();
            _mockOptions = new Mock<IOptions<ApiOptions>>();
            _mockRateService = new Mock<IRateService>();

            InitializeMocks();

            _controller = new ExchangeController(_mockRateService.Object, _mockOptions.Object, mockLogger.Object);
        }

        private void InitializeMocks()
        {
            _mockOptions.SetupGet(x => x.Value)
                .Returns(new ApiOptions
                {
                    SupportedCurrencies = new CurrencyInfo[]
                    {
                        new() { Currency = "USD", Limit = 200 },
                        new() { Currency = "BRL", Limit = 300 }
                    }
                });

            _mockRateService.Setup(x => x.GetExchangeRateAsync("USD"))
                .ReturnsAsync(new RateResponseDto
                {
                    Currency = "USD",
                    RateDate = DateTime.Now,
                    Buy = 89,
                    Sell = 91
                })
                .Verifiable();

            _mockRateService.Setup(x => x.GetExchangeRateAsync("BRL"))
                .ReturnsAsync(new RateResponseDto
                {
                    Currency = "BRL",
                    RateDate = DateTime.Now,
                    Buy = 22,
                    Sell = 23
                })
                .Verifiable();
        }

        [Fact]
        public async Task GetCurrencyRateAsync_With_Ok_Response()
        {
            var result = await _controller.GetCurrencyRateAsync("USD");

            var createdResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<RateResponseDto>(createdResult.Value);
            Assert.Equal("USD", returnValue.Currency);
        }

        [Fact]
        public async Task GetCurrencyRateAsync_With_Wrong_Currency()
        {
            try
            {
                var result = await _controller.GetCurrencyRateAsync("RUB");
            }
            catch (Exception e)
            {
                Assert.IsType<BadRequestException>(e);
                Assert.Equal("API supports the following currencies: USD, BRL", e.Message);
                Assert.Equal("Exchange.API", e.Source);
            }
        }
    }
}
