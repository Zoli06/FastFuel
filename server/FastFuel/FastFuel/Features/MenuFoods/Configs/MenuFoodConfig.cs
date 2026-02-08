using FastFuel.Features.MenuFoods.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.MenuFoods.Configs;

public class MenuFoodConfig : IEntityTypeConfiguration<MenuFood>
{
    public void Configure(EntityTypeBuilder<MenuFood> builder)
    {
        builder.HasKey(mf => new { mf.MenuId, mf.FoodId });
    }
}