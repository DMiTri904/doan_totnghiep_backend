using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Database.ConfigEntity
{
    public class GroupConfig : IEntityTypeConfiguration<Groups>
    {
        public void Configure(EntityTypeBuilder<Groups> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.Description)
                .HasMaxLength(1000);

            builder.Property(g => g.SubjectOrProjectName)
                .HasMaxLength(200);

            builder.Property(g => g.CreatedBy)
                .IsRequired();

            builder.Property(g => g.CreatedAt)
                .IsRequired();

            builder.Property(g => g.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Creator
            builder.HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Members
            builder.HasMany(g => g.Members)
                .WithOne(gm => gm.Group)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tasks
            builder.HasMany(g => g.Tasks)
                .WithOne(t => t.Groups)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // ActivityLogs
            builder.HasMany(g => g.ActivityLogs)
                .WithOne(a => a.Group)
                .HasForeignKey(a => a.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reports
            builder.HasMany(g => g.Reports)
                .WithOne(r => r.Group)
                .HasForeignKey(r => r.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
