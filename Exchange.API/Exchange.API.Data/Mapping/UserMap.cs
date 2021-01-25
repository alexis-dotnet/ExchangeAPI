using Exchange.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exchange.API.Data.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User", "dbo");
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId).ValueGeneratedOnAdd().IsRequired().HasColumnName("UserId");
            builder.Property(x => x.Name).HasColumnName("Name");

            builder.HasMany(x => x.Purchases)
                .WithOne(x => x.User);
        }
    }
}
