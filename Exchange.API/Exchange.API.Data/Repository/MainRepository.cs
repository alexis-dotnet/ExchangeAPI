using System.Threading.Tasks;
using Exchange.API.Data.Context;

namespace Exchange.API.Data.Repository
{
    public interface IRepository
    {
        Task<int> Commit();
    }

    public class MainRepository : IRepository
    {
        protected readonly MainContext Context;

        public MainRepository(MainContext context)
        {
            Context = context;
        }

        public async Task<int> Commit()
        {
            return await Context.SaveChangesAsync();
        }
    }
}
