using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class ShippingMethodDbConfiguration : IEntityTypeConfiguration<ShippingMethod>
{
    public void Configure(EntityTypeBuilder<ShippingMethod> builder)
    {
        builder.HasKey(x => x.Name);
        builder.HasMany(x => x.Countries)
            .WithMany();

        builder.Property(x => x.ApplicableFees).IsRequired();
    }
}

