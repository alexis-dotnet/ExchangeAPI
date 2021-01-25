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
    public class PurchaseControllerTests
    {
        private readonly PurchasesController _controller;
        private readonly Mock<IPurchaseService> _mockPurchasesService;
        private readonly Mock<IOptions<ApiOptions>> _mockOptions;
        private PurchaseRequestDto _okCase;
        private PurchaseRequestDto _wrongCurrencyCase;
        private PurchaseRequestDto _wrongNegativeAmountCase;
        private PurchaseRequestDto _wrongUserCase;
        private PurchaseRequestDto _wrongExchangeCase;

        public PurchaseControllerTests()
        {
            var mockLogger = new Mock<ILogger<PurchasesController>>();
            _mockOptions = new Mock<IOptions<ApiOptions>>();
            _mockPurchasesService = new Mock<IPurchaseService>();

            InitializeMocks();

            _controller = new PurchasesController(_mockPurchasesService.Object, _mockOptions.Object, mockLogger.Object);
        }

        private void InitializeMocks()
        {
            _okCase = new PurchaseRequestDto
            {
                OriginAmount = 100,
                TargetCurrency = "USD",
                UserId = 1
            };
            _wrongCurrencyCase = new PurchaseRequestDto
            {
                OriginAmount = 100,
                TargetCurrency = "RUB",
                UserId = 1
            };
            _wrongNegativeAmountCase = new PurchaseRequestDto
            {
                OriginAmount = -100,
                TargetCurrency = "USD",
                UserId = 1
            };
            _wrongUserCase = new PurchaseRequestDto
            {
                OriginAmount = 100,
                TargetCurrency = "USD",
                UserId = 1000
            };
            _wrongExchangeCase = new PurchaseRequestDto
            {
                OriginAmount = 1000000,
                TargetCurrency = "USD",
                UserId = 1
            };

            _mockOptions.SetupGet(x => x.Value)
                .Returns(new ApiOptions
                {
                    SupportedCurrencies = new CurrencyInfo[]
                    {
                        new() { Currency = "USD", Limit = 200 },
                        new() { Currency = "BRL", Limit = 300 }
                    }
                });

            _mockPurchasesService.Setup(x => x.SavePurchaseAsync(_okCase))
                .ReturnsAsync(new PurchaseResponseDto
                {
                    Currency = "USD",
                    PurchasedAmount = (decimal)1.01
                })
                .Verifiable();

            _mockPurchasesService.Setup(x => x.SavePurchaseAsync(_wrongUserCase))
                .ThrowsAsync(new BadRequestException("The requesting user doesn't exist."));

            _mockPurchasesService.Setup(x => x.SavePurchaseAsync(_wrongExchangeCase))
                .ThrowsAsync(new BadRequestException(
                    "Exchange Amount is not valid",
                    null,
                    "The exchange amount to purchase exceeds the monthly limit"));
        }

        [Fact]
        public async Task PurchaseAsync_With_Ok_Response()
        {
            var result = await _controller.PurchaseAsync(_okCase);

            var createdResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PurchaseResponseDto>(createdResult.Value);
            Assert.Equal("USD", returnValue.Currency);
            Assert.Equal((decimal) 1.01, returnValue.PurchasedAmount);
        }

        [Fact]
        public async Task PurchaseAsync_With_Wrong_Currency_Case()
        {
            try
            {
                var result = await _controller.PurchaseAsync(_wrongCurrencyCase);
            }
            catch (Exception e)
            {
                Assert.IsType<BadRequestException>(e);
                Assert.Equal("API supports the following currencies: USD, BRL", e.Message);
                Assert.Equal("Exchange.API", e.Source);
            }
        }

        [Fact]
        public async Task PurchaseAsync_With_Wrong_Negative_Amount_Case()
        {
            try
            {
                var result = await _controller.PurchaseAsync(_wrongNegativeAmountCase);
            }
            catch (Exception e)
            {
                Assert.IsType<BadRequestException>(e);
                Assert.Equal("The amount to purchase cannot be negative", e.Message);
                Assert.Equal("Exchange.API", e.Source);
            }
        }

        [Fact]
        public async Task PurchaseAsync_With_Wrong_User_Case()
        {
            try
            {
                var result = await _controller.PurchaseAsync(_wrongUserCase);
            }
            catch (Exception e)
            {
                Assert.IsType<BadRequestException>(e);
                Assert.Equal("The requesting user doesn't exist.", e.Message);
            }
        }

        [Fact]
        public async Task PurchaseAsync_With_Wrong_Exchange_Case()
        {
            try
            {
                var result = await _controller.PurchaseAsync(_wrongExchangeCase);
            }
            catch (Exception e)
            {
                Assert.IsType<BadRequestException>(e);
                Assert.Equal("The exchange amount to purchase exceeds the monthly limit", e.Message);
            }
        }
    }
}
