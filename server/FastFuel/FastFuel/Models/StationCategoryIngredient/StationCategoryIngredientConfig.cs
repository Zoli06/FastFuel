using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class StationCategoryIngredientConfig : IEntityTypeConfiguration<StationCategoryIngredient>
{
    public void Configure(EntityTypeBuilder<StationCategoryIngredient> builder)
    {
        builder.HasKey(st => new { st.StationCategoryId, st.IngredientId });
    }
}