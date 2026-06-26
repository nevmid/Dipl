using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("genres");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .HasColumnName("id");

            builder.Property(g => g.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}