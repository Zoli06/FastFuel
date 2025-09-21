using Microsoft.EntityFrameworkCore;

namespace FastFuel.Models;

public class MenuFoodConfig : IEntityTypeConfiguration<MenuFood>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<MenuFood> builder)
    {
        builder.HasKey(mf => new { mf.MenuId, mf.FoodId });
        builder.HasIndex(mf => new { mf.MenuId, mf.FoodId }).IsUnique();
        builder.Property(mf => mf.Quantity);
    }
}