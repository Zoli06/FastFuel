using FastFuel.Features.Menus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Menus.Configs;

public class MenuConfig : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasIndex(m => m.Name).IsUnique();
        builder.Property(m => m.Name).HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(600);
    }
}