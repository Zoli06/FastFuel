using FastFuel.Features.Themes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Themes.Configs
{
    public class ThemeConfiguration : IEntityTypeConfiguration<Theme>
    {
        public void Configure(EntityTypeBuilder<Theme> builder)
        {
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Background)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(t => t.Footer)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(t => t.ButtonPrimary)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(t => t.ButtonSecondary)
                .IsRequired()
                .HasMaxLength(7);
        }
    }
}