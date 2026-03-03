using FastFuel.Features.OrderFoods.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.OrderFoods.Configs;

public class OrderFoodConfig : IEntityTypeConfiguration<OrderFood>
{
    public void Configure(EntityTypeBuilder<OrderFood> builder)
    {
        builder.Property(of => of.SpecialInstructions).HasMaxLength(300);
    }
}