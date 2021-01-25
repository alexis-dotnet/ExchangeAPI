namespace Exchange.API.Dto.Exchange
{
    public class PurchaseRequestDto
    {
        public int UserId { get; set; }
        public decimal OriginAmount { get; set; }
        public string TargetCurrency { get; set; }
    }
}
