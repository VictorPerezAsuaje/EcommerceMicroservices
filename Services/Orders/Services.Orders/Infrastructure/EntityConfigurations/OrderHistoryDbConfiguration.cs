using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class OrderHistoryDbConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChangeDate).IsRequired();
        builder.Property(x => x.Message).IsRequired();
        builder.HasOne(x => x.Order)
                .WithMany(x => x.StatusHistory)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.SetNull);
    }
}

