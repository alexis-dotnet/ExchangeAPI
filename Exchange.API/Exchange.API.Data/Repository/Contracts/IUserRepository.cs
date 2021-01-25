using Exchange.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exchange.API.Data.Repository.Contracts
{
    public interface IUserRepository
    {
        DbSet<User> Users { get; }
    }
}
