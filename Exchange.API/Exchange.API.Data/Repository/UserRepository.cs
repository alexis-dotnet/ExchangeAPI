using Exchange.API.Data.Context;
using Exchange.API.Data.Entities;
using Exchange.API.Data.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Exchange.API.Data.Repository
{
    public class UserRepository : MainRepository, IUserRepository
    {
        public UserRepository(MainContext context) : base(context) { }
        public DbSet<User> Users => Context.Users;
    }
}
