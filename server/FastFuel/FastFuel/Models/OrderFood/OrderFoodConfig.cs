using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class OrderFoodConfig : IEntityTypeConfiguration<OrderFood>
{
    public void Configure(EntityTypeBuilder<OrderFood> builder)
    {
        // Uncomment if we decide to save OrderMenuId in OrderFood
        // builder.HasOne<OrderMenu>()
        //     .WithMany(om => om.OrderFoods)
        //     .HasForeignKey(of => new { of.OrderMenuId, of.OrderId })
        //     .HasPrincipalKey(om => new { om.Id, om.OrderId });
        // builder.Ignore(of => of.OrderMenu);
    }
}