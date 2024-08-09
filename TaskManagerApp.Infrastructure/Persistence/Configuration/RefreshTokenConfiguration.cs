using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Domain.Entities;

namespace TaskManagerApp.Infrastructure.Persistence.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt =>  rt.Id);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(rt => rt.ExpiryDate)
                .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                .HasDefaultValue(false);

            builder.HasOne(rt => rt.User)
             .WithOne(u => u.RefreshToken)
             .HasForeignKey<RefreshToken>(rt => rt.UserId)
             .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
