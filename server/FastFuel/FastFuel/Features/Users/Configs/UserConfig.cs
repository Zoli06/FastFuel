using FastFuel.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Users.Configs;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.UserName).HasMaxLength(50);
        builder.Property(u => u.Email).HasMaxLength(100);
        builder.Property(u => u.Name).HasMaxLength(100);
        builder.Property(u => u.PasswordHash).HasMaxLength(255);

        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}