using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerApp.Domain.Entities;

namespace TaskManagerApp.Infrastructure.Persistence.Configuration
{
    public class TaskOperationRequestConfiguration : IEntityTypeConfiguration<TaskOperationRequest>
    {
        public void Configure(EntityTypeBuilder<TaskOperationRequest> builder)
        {
            builder.HasKey(tor => tor.Id);

            builder.Property(t => t.Type)
               .IsRequired()
               .HasConversion(
                   v => v.ToString(),
                   v => (Domain.Enums.RequestType)Enum.Parse(typeof(Domain.Enums.RequestType), v));

            builder.Property(tor => tor.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne( tor => tor.User)
                .WithMany(u => u.TaskOperationRequests)
                .HasForeignKey(tor => tor.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tor => tor.Task)
                .WithMany(t => t.TaskOperationRequests)
                .HasForeignKey(tor => tor.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(tor => tor.Project)
                .WithMany()
                .HasForeignKey(tor => tor.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
