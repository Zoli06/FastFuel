using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Models;

public class SpecialRequestConfig : IEntityTypeConfiguration<SpecialRequest>
{
    public void Configure(EntityTypeBuilder<SpecialRequest> builder)
    {
        builder.Property(sr => sr.Note).HasMaxLength(200);
    }
}