using FastFuel.Features.OpeningHours.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.OpeningHours.Configs;

public class OpeningHourConfig : IEntityTypeConfiguration<OpeningHour>
{
    public void Configure(EntityTypeBuilder<OpeningHour> builder)
    {
        builder.HasIndex(b => new { b.RestaurantId, b.DayOfWeek }).IsUnique();
    }
}