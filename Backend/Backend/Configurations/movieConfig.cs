using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("movies");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("id");

            builder.Property(m => m.Title)
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(m => m.OriginalTitle)
                .HasColumnName("original_title")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasColumnName("description")
                .IsRequired();

            builder.Property(m => m.Year)
                .HasColumnName("year")
                .IsRequired();

            builder.Property(m => m.Duration)
                .HasColumnName("duration")
                .HasAnnotation("CheckConstraint", "duration > 0 AND duration <= 1000")
                .IsRequired();

            builder.Property(m => m.PosterUrl)
                .HasColumnName("poster_url")
                .HasDefaultValue("/uploads/posters/default.png");

            builder.Property(m => m.Rating)
                .HasColumnName("rating")
                .IsRequired()
                .HasAnnotation("CheckConstraint", "rating >= 0 AND rating <= 10")
                .HasColumnType("decimal(3,1)");

            builder.Property(m => m.TrailerUrl)
                .HasColumnName("trailer_url")
                .IsRequired();

            builder.HasMany(m => m.Sessions)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.Title)
                .HasDatabaseName("idx_movies_title");
        }
    }
}
