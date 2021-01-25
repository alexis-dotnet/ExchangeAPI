using System.Linq;
using Exchange.API.Data.Context;
using Exchange.API.Data.Entities;

namespace Exchange.API.Data.Seed
{
    public static class DbInitializer
    {
        public static void Initialize(MainContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
                return;

            var users = new User[]
            {
                new User { Name = "Alexis Alulema"},
                new User { Name = "Elon Musk"},
                new User { Name = "Juana Azurduy"},
                new User { Name = "Woody Allen"},
                new User { Name = "Isabel Allende"}
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
