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

            builder.Property(p => p.StatusId)
                .HasColumnName("status_id")
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(p => p.TransactionId)
                .HasColumnName("transaction_id")
                .HasMaxLength(100);

            builder.Property(p => p.PaymentUrl)
                .HasColumnName("payment_url");

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at");

            builder.HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Status)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.TransactionId);
        }
    }
}
