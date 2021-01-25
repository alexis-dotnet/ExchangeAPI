using System;

namespace Exchange.API.Dto.Exchange
{
    public class RateResponseDto
    {
        public string Currency { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
        public DateTime RateDate { get; set; }
    }
}
