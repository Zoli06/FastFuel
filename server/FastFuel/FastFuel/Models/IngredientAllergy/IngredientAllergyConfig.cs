using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models.IngredientAllergies;

public class IngredientAllergyConfig : IEntityTypeConfiguration<IngredientAllergy>
{
    public void Configure(EntityTypeBuilder<IngredientAllergy> builder)
    {
        builder.HasIndex(ia => new { ia.IngredientId, ia.AllergyId }).IsUnique();
    }
}