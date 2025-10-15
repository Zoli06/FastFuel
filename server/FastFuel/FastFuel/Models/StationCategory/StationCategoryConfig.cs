using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class StationCategoryConfig : IEntityTypeConfiguration<StationCategory>
{
    public void Configure(EntityTypeBuilder<StationCategory> builder)
    {
        builder.HasIndex(st => st.Name).IsUnique();
        builder.Property(st => st.Name).HasMaxLength(100);
    }
}