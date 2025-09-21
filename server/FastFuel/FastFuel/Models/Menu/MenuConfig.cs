using Microsoft.EntityFrameworkCore;

namespace FastFuel.Models;

public class MenuConfig : IEntityTypeConfiguration<Menu>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Menu> builder)
    {
        builder.HasIndex(m => m.Name).IsUnique();
        builder.Property(m => m.Name).HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(600);
    }
}