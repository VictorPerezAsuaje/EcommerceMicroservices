using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class OrderDbConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClientId).IsRequired();
        builder.HasMany(x => x.StatusHistory)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ShippingFirstName).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.ShippingLastName).IsRequired().HasMaxLength(1000);

        builder.OwnsOne(x => x.ShippingAddress, sa =>
        {
            sa.HasOne(s => s.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryName)
                .OnDelete(DeleteBehavior.SetNull);

            sa.Property(s => s.MajorDivision).IsRequired().HasMaxLength(1000);
            sa.Property(s => s.MajorDivisionCode).HasMaxLength(100).HasDefaultValue(Address.NotApplicable);

            sa.Property(s => s.Locality).IsRequired().HasMaxLength(1000);
            sa.Property(s => s.MinorDivision).HasMaxLength(100).HasDefaultValue(Address.NotApplicable);

            sa.Property(s => s.Street).IsRequired().HasMaxLength(1000);
            sa.Property(s => s.StreetNumber).IsRequired().HasMaxLength(100).HasDefaultValue(Address.NotApplicable);
            sa.Property(s => s.HouseNumber).IsRequired().HasMaxLength(100).HasDefaultValue(Address.NotApplicable);
            sa.Property(s => s.PostalCode).IsRequired().HasMaxLength(100).HasDefaultValue(Address.NotApplicable);

            sa.Property(s => s.Latitude).IsRequired();
            sa.Property(s => s.Longitude).IsRequired();
            sa.Property(s => s.ExtraDetails).HasMaxLength(1000);
        });

        builder.Property(x => x.ShippingMethod).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ShippingFees).IsRequired().HasMaxLength(2000);

        builder.HasOne(x => x.DiscountCodeApplied).WithOne(x => x.Order).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.SubTotal).IsRequired();
        builder.Property(x => x.TaxApplied).IsRequired();
        builder.Property(x => x.Total).IsRequired();
        builder.Property(x => x.OrderDate).IsRequired();

        builder.HasOne(x => x.Shipping)
            .WithMany()
            .HasForeignKey(x => x.ShippingMethodName)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.PaymentMethod)
           .WithMany()
           .HasForeignKey(x => x.PaymentMethodName)
           .OnDelete(DeleteBehavior.NoAction);
    }
}

