using System;
using System.Linq;
using System.Threading.Tasks;
using Exchange.API.Data.Context;
using Exchange.API.Data.Entities;
using Exchange.API.Data.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Exchange.API.Data.Repository
{
    public class PurchaseRepository : MainRepository, IPurchaseRepository
    {
        public PurchaseRepository(MainContext context) : base(context) { }

        public DbSet<Purchase> Purchases => Context.Purchases;

        public async Task<decimal> TotalPurchasesInMonthAsync(int userId, string currency)
        {
            var total = await Purchases
                .Where(x => x.UserId == userId && x.TransactionDate > DateTime.Now.AddMonths(-1) &&
                            x.TargetCurrency == currency)
                .SumAsync(x => x.TargetAmount);

            return total;
        }

        public async Task<int> CommitAsync()
        {
            return await Commit();
        }
    }
}
