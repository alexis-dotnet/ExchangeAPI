using System.Linq;
using System.Threading.Tasks;
using Exchange.API.Dto.Exchange;
using Exchange.API.Services.Common;
using Exchange.API.Services.Contracts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exchange.API.Controllers
{
    /// <summary>
    /// Gives the exchange rates for USD and BRL currencies, and allows the user to make currency purchases
    /// </summary>
    [Route("api/exchange")]
    [ApiController]
    [EnableCors("api-exchange-policy")]
    public class ExchangeController : ControllerBase
    {
        /// <summary>
        /// API Options from appsettings.json
        /// </summary>
        private readonly ApiOptions _settings;
        private readonly IRateService _rateService;
        private readonly ILogger _logger;

        /// <summary>
        /// Gives the exchange rates for USD and BRL currencies, and allows the user to make currency purchases
        /// </summary>
        /// <param name="rateService"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public ExchangeController(IRateService rateService, IOptions<ApiOptions> settings, ILogger<ExchangeController> logger)
        {
            _rateService = rateService;
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the current currency rate
        /// </summary>
        /// <param name="isoCode">ISO code for the currency requested. Expected 3-characters length.</param>
        /// <returns></returns>
        [HttpGet("rate/{isoCode}", Name = "GetCurrencyRate")]
        [ProducesResponseType(typeof(RateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrencyRateAsync(string isoCode)
        {
            var isoCodes = _settings
                .SupportedCurrencies
                .Select(x => x.Currency)
                .ToList();

            if (!isoCodes.Contains(isoCode.ToUpper()))
            {
                _logger.LogInformation($"Bad request. Source : {isoCode}");
                throw new BadRequestException($"API supports the following currencies: {string.Join(", ", isoCodes)}");
            }

            var response = await _rateService.GetExchangeRateAsync(isoCode);
            _logger.LogInformation($"Successful request. Source : {isoCode}");
            return Ok(response);
        }
    }
}
