using FastFuel.Features.Ingredients.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Ingredients.Configs;

public class IngredientConfig : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.Property(i => i.Name).HasMaxLength(100);
        builder.HasIndex(i => i.Name).IsUnique();
    }
}