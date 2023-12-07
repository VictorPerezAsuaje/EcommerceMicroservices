using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class OrderHistoryItemDbConfiguration : IEntityTypeConfiguration<OrderHistoryItem>
{
    public void Configure(EntityTypeBuilder<OrderHistoryItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChangeDate).IsRequired();
        builder.Property(x => x.Message).IsRequired();
    }
}



