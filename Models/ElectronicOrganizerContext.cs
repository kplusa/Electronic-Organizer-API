using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Electronic_Organizer_API.Models
{
    public partial class ElectronicOrganizerContext : DbContext
    {
        public ElectronicOrganizerContext()
        {
        }

        public ElectronicOrganizerContext(DbContextOptions<ElectronicOrganizerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EndUser> EndUsers { get; set; }
        public virtual DbSet<EndUserSecurity> EndUserSecurities { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EndUser>(entity =>
            {
                entity.ToTable("end_user");

                entity.HasIndex(e => e.Email, "UQ__end_user__AB6E6164707D2203")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .IsUnicode(false)
                    .HasColumnName("avatar");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");
            });

            modelBuilder.Entity<EndUserSecurity>(entity =>
            {
                entity.ToTable("end_user_security");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EndUserId).HasColumnName("end_user_id");

                entity.Property(e => e.HashedPassword)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("hashed_password");

                entity.Property(e => e.Salt)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("salt");

                entity.HasOne(d => d.EndUser)
                    .WithMany(p => p.EndUserSecurities)
                    .HasForeignKey(d => d.EndUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__end_user___end_u__4D5F7D71");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("event");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.EndTime).HasColumnName("end_time");

                entity.Property(e => e.EndUserId).HasColumnName("end_user_id");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.StartTime).HasColumnName("start_time");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.EndUser)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.EndUserId)
                    .HasConstraintName("FK__event__end_user___11158940");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__event__service_i__10216507");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("service");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.EndUserId).HasColumnName("end_user_id");

                entity.Property(e => e.EstimatedTime).HasColumnName("estimated_time");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.EndUser)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.EndUserId)
                    .HasConstraintName("FK__service__end_use__74794A92");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
