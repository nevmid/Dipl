using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.BookingId)
                .HasColumnName("booking_id")
                .IsRequired();

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasColumnName("status")
                .HasDefaultValue("pending")
                .HasAnnotation("CheckConstraint",
                    "status IN ('pending', 'confirmed', 'cancelled')");

            builder.HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
