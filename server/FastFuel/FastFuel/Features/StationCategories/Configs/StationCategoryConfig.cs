using FastFuel.Features.StationCategories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.StationCategories.Configs;

public class StationCategoryConfig : IEntityTypeConfiguration<StationCategory>
{
    public void Configure(EntityTypeBuilder<StationCategory> builder)
    {
        builder.HasIndex(st => st.Name).IsUnique();
        builder.Property(st => st.Name).HasMaxLength(100);
    }
}