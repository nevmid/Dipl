using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("seats");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasColumnName("id");

            builder.Property(s => s.HallId)
                .HasColumnName("hall_id")
                .IsRequired();

            builder.Property(s => s.RowNum)
                .HasColumnName("row_num")
                .IsRequired();

            builder.Property(s => s.ColNum)
               .HasColumnName("col_num")
               .IsRequired();

            builder.Property(s => s.Type)
                .HasColumnName("type")
                .HasDefaultValue("standard")
                .HasAnnotation("CheckConstraint", "type in ('standard', 'vip')")
                .IsRequired();

            builder.HasOne(s => s.Hall)
                .WithMany(h =>  h.Seats)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.BookingSeats)
                .WithOne(bs => bs.Seat)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
