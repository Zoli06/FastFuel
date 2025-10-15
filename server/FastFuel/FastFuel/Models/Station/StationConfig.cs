using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class StationConfig : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(100);
        builder.HasIndex(s => new { s.RestaurantId, s.Name }).IsUnique();
        builder.HasIndex(s => new { s.RestaurantId, s.StationCategoryId });
    }
}