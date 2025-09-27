using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class FoodIngredientConfig : IEntityTypeConfiguration<FoodIngredient>
{
    public void Configure(EntityTypeBuilder<FoodIngredient> builder)
    {
        builder.HasKey(fi => new { fi.FoodId, fi.IngredientId });
        builder.Property(fi => fi.Quantity);
    }
}