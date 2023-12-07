using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class OrderHistoryDbConfiguration : IEntityTypeConfiguration<OrderHistory>
{
    public void Configure(EntityTypeBuilder<OrderHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(os => os.Statuses)
           .WithOne(o => o.OrderHistory)
           .HasForeignKey(os => os.OrderHistoryId)
           .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(os => os.Order)
           .WithOne(x => x.History)
           .HasForeignKey<Order>(x => x.HistoryId)
           .OnDelete(DeleteBehavior.ClientNoAction);
    }
}



