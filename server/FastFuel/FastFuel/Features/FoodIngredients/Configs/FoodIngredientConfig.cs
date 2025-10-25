using FastFuel.Features.FoodIngredients.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.FoodIngredients.Configs;

public class FoodIngredientConfig : IEntityTypeConfiguration<FoodIngredient>
{
    public void Configure(EntityTypeBuilder<FoodIngredient> builder)
    {
        builder.HasIndex(fi => new { fi.FoodId, fi.IngredientId }).IsUnique();
    }
}