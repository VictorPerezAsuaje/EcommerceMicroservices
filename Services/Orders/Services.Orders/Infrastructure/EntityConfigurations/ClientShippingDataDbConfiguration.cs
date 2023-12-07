using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Orders.Domain;

namespace Services.Orders.Infrastructure.EntityConfigurations;

internal class ClientShippingDataDbConfiguration : IEntityTypeConfiguration<ClientShippingData>
{
    public void Configure(EntityTypeBuilder<ClientShippingData> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ClientId).IsRequired();
        builder.Property(x => x.ShippingFirstName).IsRequired();
        builder.Property(x => x.ShippingLastName).IsRequired();
        builder.OwnsOne(x => x.ShippingAddress, sa =>
        {
            sa.HasOne(s => s.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryName)
                .OnDelete(DeleteBehavior.ClientNoAction);

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
    }
}

