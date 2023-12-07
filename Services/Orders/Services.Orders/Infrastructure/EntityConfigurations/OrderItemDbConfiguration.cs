using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class OrderItemDbConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => new { x.OrderId, x.ProductId });
        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
    }
}

