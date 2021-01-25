using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exchange.API.Data.Entities;
using Exchange.API.Data.Repository.Contracts;
using Exchange.API.Dto.Exchange;
using Exchange.API.Services.Common;
using Exchange.API.Services.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Exchange.API.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IRateService _rateService;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApiOptions _settings;
        private readonly ILogger _logger;

        public PurchaseService(IRateService rateService, IUserRepository userRepository, IPurchaseRepository purchaseRepository, IOptions<ApiOptions> settings, ILogger<PurchaseService> logger)
        {
            _rateService = rateService;
            _purchaseRepository = purchaseRepository;
            _settings = settings.Value;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PurchaseResponseDto> SavePurchaseAsync(PurchaseRequestDto request)
        {
            var user = _userRepository.Users.FirstOrDefault(x => x.UserId == request.UserId);

            if (user == null)
            {
                _logger.LogDebug($"User not found: {request.UserId}");
                throw new BadRequestException("The requesting user doesn't exist.");
            }

            var rate = await  _rateService.GetExchangeRateAsync(request.TargetCurrency);
            var currencyInfo = _settings.SupportedCurrencies.FirstOrDefault(x => x.Currency == request.TargetCurrency);
            decimal estimatedTargetAmount = Math.Round(request.OriginAmount / rate.Sell, 2);
            decimal totalInMonth = await _purchaseRepository.TotalPurchasesInMonthAsync(request.UserId, request.TargetCurrency);

            if (estimatedTargetAmount + totalInMonth > currencyInfo.Limit)
            {
                _logger.LogDebug($"Exchange Amount exceeds the limit. Source: {JsonConvert.SerializeObject(request)}");
                throw new BadRequestException(
                    "Exchange Amount is not valid",
                    new Dictionary<string, string[]>
                    {
                        {"Limit has been exceeded", new []
                        {
                            $"You have tried to buy {estimatedTargetAmount} {currencyInfo.Currency}",
                            $"This month you still can buy {currencyInfo.Limit - totalInMonth} {currencyInfo.Currency}",
                            $"The monthly limit of {currencyInfo.Currency} to be purchased is {currencyInfo.Limit}",
                            $"The current conversion rate at the moment is {rate.Sell} ARS per 1 {currencyInfo.Currency}",
                            $"The available value you can exchange is {Math.Round((currencyInfo.Limit - totalInMonth) * rate.Sell, 2)} ARS."
                        }},
                    },
                    "The exchange amount to purchase exceeds the monthly limit");
            }

            var purchase = new Purchase
            {
                UserId = request.UserId,
                OriginAmount = request.OriginAmount,
                TargetAmount = estimatedTargetAmount,
                TargetCurrency = request.TargetCurrency,
            };

            await _purchaseRepository.Purchases.AddAsync(purchase);
            int response = await _purchaseRepository.CommitAsync();

            if (response == 0)
            {
                _logger.LogCritical("Internal database error has avoided to save the transaction");
                throw new Exception("Internal database error has avoided to save the transaction");
            }

            _logger.LogInformation($"Successful transaction.");
            return new PurchaseResponseDto
            {
                Currency = currencyInfo.Currency,
                PurchasedAmount = estimatedTargetAmount
            };
        }
    }
}
