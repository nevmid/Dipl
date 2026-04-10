using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Backend.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("sessions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasColumnName("id");

            builder.Property(s => s.MovieId)
                .HasColumnName("movie_id")
                .IsRequired();

            builder.Property(s => s.HallId)
                .HasColumnName("hall_id")
                .IsRequired();

            builder.Property(s => s.StartTime)
                .HasColumnName("start_time")
                .IsRequired();

            builder.Property(s => s.EndTime)
                .HasColumnName("end_time")
                .IsRequired();

            builder.Property(s => s.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price")
                .IsRequired();

            builder.HasOne(s => s.Movie)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s =>  s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Bookings)
                .WithOne(b => b.Session)
                .HasForeignKey(b => b.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(s => s.HasCheckConstraint(
                "ck_sessions_times_valid",
                "end_time > start_time"));
        }
    }
}
