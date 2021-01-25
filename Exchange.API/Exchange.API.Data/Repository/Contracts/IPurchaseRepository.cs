using System.Threading.Tasks;
using Exchange.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exchange.API.Data.Repository.Contracts
{
    public interface IPurchaseRepository
    {
        DbSet<Purchase> Purchases { get; }
        Task<int> CommitAsync();
        Task<decimal> TotalPurchasesInMonthAsync(int userId, string currency);
    }
}
