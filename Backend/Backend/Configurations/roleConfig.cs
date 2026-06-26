using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("id");

            builder.Property(r => r.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(r => r.Name)
                .IsUnique();

            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Role { Id = 1, Name = "user" },
                new Role { Id = 2, Name = "admin" },
                new Role { Id = 3, Name = "staff" }
            );
        }
    }
}
