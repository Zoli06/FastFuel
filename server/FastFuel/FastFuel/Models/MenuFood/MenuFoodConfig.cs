using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class MenuFoodConfig : IEntityTypeConfiguration<MenuFood>
{
    public void Configure(EntityTypeBuilder<MenuFood> builder)
    {
        builder.HasIndex(mf => new { mf.MenuId, mf.FoodId }).IsUnique();
    }
}