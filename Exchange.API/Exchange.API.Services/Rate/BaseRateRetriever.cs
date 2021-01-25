using System.Threading.Tasks;
using Exchange.API.Dto.Exchange;
using Exchange.API.Services.Common;

namespace Exchange.API.Services.Rate
{
    public interface IRateRetriever
    {
        Task<RateResponseDto> GetRateAsync();
    }

    public abstract class BaseRateRetriever
    {
        protected readonly ApiOptions Settings;
        protected readonly IHttpCallService HttpService;

        protected BaseRateRetriever(ApiOptions settings, IHttpCallService httpService)
        {
            Settings = settings;
            HttpService = httpService;
        }
    }
}
