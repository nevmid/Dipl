using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class BarCategoryConfiguration : IEntityTypeConfiguration<BarCategory>
    {
        public void Configure(EntityTypeBuilder<BarCategory> builder)
        {
            builder.ToTable("bar_categories");

            builder.HasKey(bc => bc.Id);

            builder.Property(bc => bc.Id)
                .HasColumnName("id");

            builder.Property(bc => bc.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(bc => bc.BarItems)
                .WithOne(bi => bi.Category)
                .HasForeignKey(bi => bi.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(bc => bc.Name)
                .IsUnique();
        }
    }
}
