using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class BookingSeatConfiguration : IEntityTypeConfiguration<BookingSeat>
    {
        public void Configure(EntityTypeBuilder<BookingSeat> builder)
        {
            builder.ToTable("booking_seats");

            builder.HasKey(bs => bs.Id);

            builder.Property(bs => bs.Id)
                .HasColumnName("id");

            builder.Property(bs => bs.BookingId)
                .HasColumnName("booking_id")
                .IsRequired();

            builder.Property(bs => bs.SeatId)
                .HasColumnName("seat_id")
                .IsRequired();

            builder.HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs =>  bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
