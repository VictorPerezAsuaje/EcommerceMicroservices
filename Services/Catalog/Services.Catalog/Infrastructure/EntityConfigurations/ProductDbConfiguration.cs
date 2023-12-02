using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalog.Domain;

namespace Services.Catalog.Infrastructure.EntityConfigurations;

internal class ProductDbConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Price).IsRequired();

        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Details).HasMaxLength(2000);

        builder.HasOne(x => x.Category).WithMany()
            .HasForeignKey(x => x.CategoryName)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Tags).WithMany();

        builder.HasMany(x => x.Reviews).WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

