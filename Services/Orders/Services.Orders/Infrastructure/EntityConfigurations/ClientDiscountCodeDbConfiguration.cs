using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class ClientDiscountCodeDbConfiguration : IEntityTypeConfiguration<ClientDiscountCode>
{
    public void Configure(EntityTypeBuilder<ClientDiscountCode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.DiscountCode)
            .WithMany()
            .HasForeignKey(x => x.Code)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.Property(x => x.ClientId).IsRequired();
        builder.Property(x => x.UsageDate).IsRequired();
        builder.HasOne(x => x.Order)
            .WithOne(x => x.DiscountCodeApplied)
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}

