using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Configurations
{
    public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
    {
        public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
        {
            builder.ToTable("loyalty_accounts");

            builder.HasKey(la => la.Id);

            builder.Property(la => la.Id)
                .HasColumnName("id");

            builder.Property(la => la.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(la => la.Balance)
                .HasColumnName("balance")
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder.HasOne(la => la.User)
                .WithOne(u => u.LoyaltyAccount)
                .HasForeignKey<LoyaltyAccount>(la => la.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(la => la.HasCheckConstraint(
                "ck_loyalty_balance_non_negative",
                "balance >= 0"));
        }
    }
}
