using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class BarItemConfiguration : IEntityTypeConfiguration<BarItem>
    {
        public void Configure(EntityTypeBuilder<BarItem> builder)
        {
            builder.ToTable("bar_items");

            builder.HasKey(ba => ba.Id);

            builder.Property(ba => ba.Id)
                .HasColumnName("id");

            builder.Property(ba => ba.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(ba => ba.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(ba => ba.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(10, 2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(ba => ba.IsAvailable)
                .HasColumnName("is_available")
                .HasDefaultValue(true)
                .IsRequired();

            builder.HasOne(ba => ba.Category)
                .WithMany(bc => bc.BarItems)
                .HasForeignKey(ba => ba.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ba => ba.Name)
                .IsUnique();
        }
    }
}
