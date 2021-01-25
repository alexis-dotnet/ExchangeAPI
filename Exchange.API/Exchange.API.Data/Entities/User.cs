using System.Collections.Generic;

namespace Exchange.API.Data.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public List<Purchase> Purchases { get; set; } = new();
    }
}
