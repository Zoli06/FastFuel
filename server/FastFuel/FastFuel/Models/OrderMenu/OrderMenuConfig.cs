using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class OrderMenuConfig : IEntityTypeConfiguration<OrderMenu>
{
    public void Configure(EntityTypeBuilder<OrderMenu> builder)
    {
        builder.Property(om => om.SpecialInstructions).HasMaxLength(300);
        // I didn't make menu unique for each order because
        // one menu could have special request and another not
    }
}