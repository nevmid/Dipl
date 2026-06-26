using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class StatusCinfiguration : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.ToTable("statuses");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");

            builder.Property(s => s.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(s => s.Bookings)
                .WithOne(b => b.Status)
                .HasForeignKey(b => b.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Payments)
               .WithOne(p => p.Status)
               .HasForeignKey(p => p.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Status { Id = 1, Name = "pending"},
                new Status { Id = 2, Name = "confirmed" },
                new Status { Id = 3, Name = "cancelled" },
                new Status { Id = 4, Name = "paid" },
                new Status { Id = 5, Name = "failed" },
                new Status { Id = 6, Name = "refunded" },
                new Status { Id = 7, Name = "active" },
                new Status { Id = 8, Name = "used" },
                new Status { Id = 9, Name = "expired" }
             );

            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}
