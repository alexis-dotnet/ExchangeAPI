using Exchange.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Exchange.API.Data.Mapping
{
    public class PurchaseMap : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchase", "dbo");
            builder.HasKey(x => x.PurchaseId);
            builder.Property(x => x.PurchaseId).ValueGeneratedOnAdd().IsRequired().HasColumnName("PurchaseId");
            builder.Property(x => x.OriginAmount).HasColumnName("OriginAmount");
            builder.Property(x => x.OriginCurrency).HasColumnName("OriginCurrency");
            builder.Property(x => x.TargetAmount).HasColumnName("TargetAmount");
            builder.Property(x => x.TargetCurrency).HasColumnName("TargetCurrency");
            builder.Property(x => x.UserId).HasColumnName("UserId");
            builder.Property(x => x.TransactionDate).HasColumnName("TransactionDate");

            builder.HasOne(x => x.User)
                .WithMany(y => y.Purchases)
                .HasForeignKey(x => x.UserId);

        }
    }
}
