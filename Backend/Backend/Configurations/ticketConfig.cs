using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");

            builder.Property(t => t.TicketNumber)
                .HasColumnName("ticket_number")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.JwtToken)
                .HasColumnName("jwt_token")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(t => t.StatusId)
                .HasColumnName("status_id")
                .HasDefaultValue(7)
                .IsRequired();

            builder.Property(t => t.BookingId)
                .HasColumnName("booking_id")
                .IsRequired();

            builder.HasIndex(t => t.TicketNumber)
                .IsUnique();

            builder.HasIndex(t => t.BookingId)
                .IsUnique();

            builder.Property(t => t.IsUsed)
                .HasColumnName("is_used")
                .HasDefaultValue(false);

            builder.Property(t => t.GeneratedAt)
                .HasColumnName("generated_at")
                .HasDefaultValueSql("NOW()");

            builder.HasOne(t => t.Booking)
                .WithOne(b => b.Ticket)
                .HasForeignKey<Ticket>(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Status)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
