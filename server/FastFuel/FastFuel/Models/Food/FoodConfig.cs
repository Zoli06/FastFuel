using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class FoodConfig : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasIndex(f => f.Name).IsUnique();
        builder.Property(f => f.Name).HasMaxLength(100);
        builder.Property(f => f.Description).HasMaxLength(600);
    }
}