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
using Newtonsoft.Json;

namespace Exchange.API.Controllers
{
    /// <summary>
    /// Allows to create purchase transactions.
    /// </summary>
    [Route("api/purchases")]
    [ApiController]
    [EnableCors("api-exchange-policy")]
    public class PurchasesController : ControllerBase
    {
        private readonly ApiOptions _settings;
        private readonly IPurchaseService _service;
        private readonly ILogger _logger;

        /// <summary>
        /// Allows to create purchase transactions.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="settings"></param>
        public PurchasesController(IPurchaseService service, IOptions<ApiOptions> settings, ILogger<PurchasesController> logger)
        {
            _service = service;
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Creates a purchase of an specified currency
        /// </summary>
        /// <param name="request">The structure of the request:
        /// <ul>
        /// <li><b>UserId:</b> The Id of one the users in table 'Users'</li>
        /// <li><b>OriginAmount:</b> The amount of ARS to be converted</li>
        /// <li><b>TargetCurrency:</b> The currency to purchase</li>
        /// </ul>
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(PurchaseResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PurchaseAsync([FromBody] PurchaseRequestDto request)
        {
            var isoCodes = _settings
                .SupportedCurrencies
                .Select(x => x.Currency)
                .ToList();

            if (!isoCodes.Contains(request.TargetCurrency.ToUpper()))
            {
                _logger.LogDebug($"Bad request. Source: {JsonConvert.SerializeObject(request)}");
                throw new BadRequestException($"API supports the following currencies: {string.Join(", ", isoCodes)}");
            }

            if (request.OriginAmount <= 0)
            {
                _logger.LogDebug($"Bad request. Source: {JsonConvert.SerializeObject(request)}");
                throw new BadRequestException("The amount to purchase cannot be negative");
            }

            var purchase = await _service.SavePurchaseAsync(request);

            return Ok(purchase);
        }
    }
}
