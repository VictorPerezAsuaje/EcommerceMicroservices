using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Catalog.Domain;

namespace Services.Catalog.Infrastructure.EntityConfigurations;

internal class ReviewDbConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Email).IsRequired();

        builder.Property(x => x.Rating).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(500);
        builder.Property(x => x.Body).HasMaxLength(2000);

        builder.HasOne(x => x.Product).WithMany(x => x.Reviews)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

