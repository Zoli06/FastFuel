using FastFuel.Features.Shifts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastFuel.Features.Shifts.Configs;

public class ShiftConfig : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.HasOne(s => s.Employee)
            .WithMany(e => e.Shifts)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}