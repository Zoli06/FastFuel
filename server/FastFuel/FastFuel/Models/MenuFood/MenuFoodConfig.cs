using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class MenuFoodConfig : IEntityTypeConfiguration<MenuFood>
{
    public void Configure(EntityTypeBuilder<MenuFood> builder)
    {
        builder.HasKey(mf => new { mf.MenuId, mf.FoodId });
        builder.Property(mf => mf.Quantity);
    }
}