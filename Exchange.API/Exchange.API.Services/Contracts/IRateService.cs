using System.Threading.Tasks;
using Exchange.API.Dto.Exchange;

namespace Exchange.API.Services.Contracts
{
    public interface IRateService
    {
        Task<RateResponseDto> GetExchangeRateAsync(string isoCode);
    }
}
