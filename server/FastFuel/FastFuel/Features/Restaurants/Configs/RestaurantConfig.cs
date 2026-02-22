using FastFuel.Features.Restaurants.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Restaurants.Configs;

public class RestaurantConfig : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.HasKey(r => r.Id);
        builder.HasIndex(r => r.Name).IsUnique();
        builder.Property(r => r.Name).HasMaxLength(100);
        builder.Property(r => r.Description).HasMaxLength(600);
        builder.Property(r => r.Address).HasMaxLength(200);
        builder.Property(r => r.Phone).HasMaxLength(15);
        builder.Property(r => r.PasswordHash).HasMaxLength(256);
    }
}