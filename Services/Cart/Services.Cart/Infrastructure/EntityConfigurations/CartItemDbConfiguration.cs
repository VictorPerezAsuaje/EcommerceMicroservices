
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Cart.Domain;

namespace Services.Cart.Infrastructure.EntityConfigurations;
internal class CartItemDbConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(x => new { x.ClientId, x.ProductId });

        builder.Property(x => x.ThumbnailUrl).HasMaxLength(2000);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Price).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.DiscountApplied).IsRequired();
    }
}