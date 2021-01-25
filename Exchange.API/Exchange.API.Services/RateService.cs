using System;
using System.Threading.Tasks;
using Exchange.API.Dto.Exchange;
using Exchange.API.Services.Common;
using Exchange.API.Services.Contracts;
using Exchange.API.Services.Rate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Exchange.API.Services
{
    public class RateService : IRateService
    {
        private readonly ApiOptions _settings;
        private readonly IHttpCallService _httpService;
        private readonly ILogger _logger;

        public RateService(IOptions<ApiOptions> settings, IHttpCallService httpService, ILogger<RateService> logger)
        {
            _settings = settings.Value;
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<RateResponseDto> GetExchangeRateAsync(string isoCode)
        {
            var type = Type.GetType($"Exchange.API.Services.Rate.{isoCode.FirstCharToUpper()}RateRetriever");
            var constructor = type?.GetConstructor(new[] { typeof(ApiOptions), typeof(HttpCallService) });

            if (constructor != null)
            {
                var retriever = (IRateRetriever) constructor.Invoke(new object[] { _settings, _httpService });

                var response = await retriever.GetRateAsync();
                response.Currency = isoCode;
                response.Buy = Math.Round(response.Buy, 2);
                response.Sell = Math.Round(response.Sell, 2);
                _logger.LogCritical($"Successful request. Source: {JsonConvert.SerializeObject(response)}");

                return response;
            }

            _logger.LogCritical("It was not possible to create an instance for the Currency Rate Retriever.");
            throw new Exception("It was not possible to create an instance for the Currency Rate Retriever.");
        }
    }
}
