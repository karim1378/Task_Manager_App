﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagerApp.Domain.Entities;

namespace TaskManagerApp.Infrastructure.Persistence.Configuration
{
    public class UserProjectConfiguration : IEntityTypeConfiguration<UserProject>
    {
        public void Configure(EntityTypeBuilder<UserProject> builder)
        {
            builder.HasKey(up => new { up.UserId, up.ProjectId });

            builder.HasOne(up => up.User)
                   .WithMany(u => u.UserProjects)
                   .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.Project)
                   .WithMany(p => p.UserProjects)
                   .HasForeignKey(up => up.ProjectId);
        }
    }
}