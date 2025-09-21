using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class AllergyConfig : IEntityTypeConfiguration<Allergy>
{
    public void Configure(EntityTypeBuilder<Allergy> builder)
    {
        builder.HasIndex(a => a.Name).IsUnique();
        builder.Property(a => a.Name).HasMaxLength(100);
        builder.Property(a => a.Message).HasMaxLength(600);
    }
}