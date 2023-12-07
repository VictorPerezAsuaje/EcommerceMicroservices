using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class DiscountCodeDbConfiguration : IEntityTypeConfiguration<DiscountCode>
{
    public void Configure(EntityTypeBuilder<DiscountCode> builder)
    {
        builder.HasKey(x => x.Code);
        builder.Property(x => x.Value).IsRequired().HasMaxLength(4);
        builder.Property(x => x.NumberOfUses).IsRequired();
        builder.Property(x => x.MaxUsage).IsRequired();
        builder.Property(x => x.MaxUsagePerClient).IsRequired();
        builder.Property(x => x.ValidUntil).IsRequired();
        builder.Property(x => x.AssociatedClientId);
    }
}

