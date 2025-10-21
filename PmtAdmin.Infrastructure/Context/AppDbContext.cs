using Microsoft.EntityFrameworkCore;
using PmtAdmin.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmtAdmin.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                // Configure primary key
                entity.HasKey(e => e.Id);

                // Configure email unique constraint
                entity.HasIndex(e => e.Email)
                      .IsUnique();

                // Configure index on is_deleted
                entity.HasIndex(e => e.Is_Deleted);

                // Set default values
                entity.Property(e => e.Is_Active)
                      .HasDefaultValue(true);

                entity.Property(e => e.Is_Super_Admin)
                      .HasDefaultValue(false);

                entity.Property(e => e.Created_At)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Is_Deleted)
                      .HasDefaultValue(false);

                // Configure self-referencing foreign keys
                entity.HasOne(e => e.CreatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.Created_By)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UpdatedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.Updated_By)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DeletedByUser)
                      .WithMany()
                      .HasForeignKey(e => e.Deleted_By)
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure string lengths
                entity.Property(e => e.Email)
                      .HasMaxLength(255);

                entity.Property(e => e.Password_Hash)
                      .HasMaxLength(1024);

                entity.Property(e => e.Name)
                      .HasMaxLength(150);

                entity.Property(e => e.Avatar_Url)
                      .HasMaxLength(1000);

                entity.Property(e => e.Jira_Id)
                      .HasMaxLength(1024);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
