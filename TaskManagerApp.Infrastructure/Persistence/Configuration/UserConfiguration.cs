using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerApp.Domain.Entities;

namespace TaskManagerApp.Infrastructure.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Email)
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.HasOne(u => u.RefreshToken)
            .WithOne(rt => rt.User)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(u => u.OwnedProjects)
               .WithOne(p => p.Owner)
               .HasForeignKey(p => p.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.UserProjects)
                   .WithOne(up => up.User)
                   .HasForeignKey(up => up.UserId);

            builder.HasMany(u => u.CreatedTasks)
                   .WithOne(t => t.Creator)
                   .HasForeignKey(t => t.CreatorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.AssignedTasks)
                   .WithOne(t => t.Assignee)
                   .HasForeignKey(t => t.AssigneeId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
