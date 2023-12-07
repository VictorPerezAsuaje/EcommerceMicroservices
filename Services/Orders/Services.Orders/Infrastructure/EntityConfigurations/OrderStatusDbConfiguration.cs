using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class OrderStatusDbConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.HasKey(x => x.Alias);
        builder.Property(x => x.Name).IsRequired();
    }
}

