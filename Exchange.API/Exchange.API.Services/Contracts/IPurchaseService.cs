using System.Threading.Tasks;
using Exchange.API.Dto.Exchange;

namespace Exchange.API.Services.Contracts
{
    public interface IPurchaseService
    {
        Task<PurchaseResponseDto> SavePurchaseAsync(PurchaseRequestDto request);
    }
}
