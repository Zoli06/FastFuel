using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class IngredientConfig : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasIndex(i => i.Name).IsUnique();
        builder.Property(i => i.Name).HasMaxLength(100);
        builder.HasIndex(i => i.StationTypeId);
    }
}