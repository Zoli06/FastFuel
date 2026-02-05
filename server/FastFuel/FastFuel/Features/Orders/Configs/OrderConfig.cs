using FastFuel.Features.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Orders.Configs;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.OrderNumber).HasMaxLength(5);
        builder.HasIndex(o => o.RestaurantId);
        builder.Property(o => o.Status).HasDefaultValue(OrderStatus.Pending);
        builder.Property(o => o.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}