namespace Exchange.API.Services.Common
{
    public class ApiOptions
    {
        public string UsdExchangeUrl { get; set; }
        public string BrlExchangeUrl { get; set; }
        public CurrencyInfo[] SupportedCurrencies { get; set; }
    }
}
