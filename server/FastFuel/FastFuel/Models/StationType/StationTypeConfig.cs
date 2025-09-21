using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class StationTypeConfig : IEntityTypeConfiguration<StationType>
{
    public void Configure(EntityTypeBuilder<StationType> builder)
    {
        builder.HasIndex(st => st.Name).IsUnique();
        builder.Property(st => st.Name).HasMaxLength(100);
    }
}