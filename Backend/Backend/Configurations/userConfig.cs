using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Backend.Models.Entities;

namespace Backend.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id");

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.RoleId)
                .HasColumnName("role_id");

            builder.HasMany(u => u.Bookings)
                .WithOne(b =>  b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.LoyaltyAccount)
                .WithOne(la => la.User)
                .HasForeignKey<LoyaltyAccount>(la => la.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
