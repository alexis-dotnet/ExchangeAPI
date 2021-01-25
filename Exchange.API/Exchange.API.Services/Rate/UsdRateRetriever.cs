using System;
using System.Globalization;
using System.Threading.Tasks;
using Exchange.API.Dto.Exchange;
using Exchange.API.Services.Common;

namespace Exchange.API.Services.Rate
{
    public class UsdRateRetriever : BaseRateRetriever, IRateRetriever
    {
        public UsdRateRetriever(ApiOptions settings, IHttpCallService httpService) : base(settings, httpService)  { }

        public async Task<RateResponseDto> GetRateAsync()
        {
            var response = await HttpService.CallEndpoint(HttpVerb.Get, new Uri(Settings.UsdExchangeUrl));
            return ParseUsdRate(response);
        }

        private RateResponseDto ParseUsdRate(string response)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var parts = response
                .Replace("[", "")
                .Replace("]", "")
                .Replace("\"", "")
                .Split(',');

            var rateResponse = new RateResponseDto
            {
                Buy = decimal.Parse(parts[0]),
                Sell = decimal.Parse(parts[1]),
                RateDate = DateTime.ParseExact(parts[2].Replace("Actualizada al ", ""), "dd/M/yyyy HH:mm", provider)
            };

            return rateResponse;
        }

    }
}
