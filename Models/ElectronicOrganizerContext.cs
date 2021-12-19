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

        public virtual DbSet<Day> Days { get; set; }
        public virtual DbSet<DayOfTimetable> DayOfTimetables { get; set; }
        public virtual DbSet<EndUser> EndUsers { get; set; }
        public virtual DbSet<EndUserSecurity> EndUserSecurities { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Timetable> Timetables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Day>(entity =>
            {
                entity.ToTable("day");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date).HasColumnName("date");
            });

            modelBuilder.Entity<DayOfTimetable>(entity =>
            {
                entity.ToTable("day_of_timetable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DayId).HasColumnName("day_id");

                entity.Property(e => e.TimetableId).HasColumnName("timetable_id");

                entity.HasOne(d => d.Day)
                    .WithMany(p => p.DayOfTimetables)
                    .HasForeignKey(d => d.DayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__day_of_ti__day_i__55009F39");

                entity.HasOne(d => d.Timetable)
                    .WithMany(p => p.DayOfTimetables)
                    .HasForeignKey(d => d.TimetableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__day_of_ti__timet__55F4C372");
            });

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

                entity.Property(e => e.DayOfTimetableId).HasColumnName("day_of_timetable_id");

                entity.Property(e => e.EndTime).HasColumnName("end_time");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.StartTime).HasColumnName("start_time");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.DayOfTimetable)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.DayOfTimetableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__event__day_of_ti__5BAD9CC8");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__event__service_i__5CA1C101");
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

            modelBuilder.Entity<Timetable>(entity =>
            {
                entity.ToTable("timetable");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EndUserId).HasColumnName("end_user_id");

                entity.HasOne(d => d.EndUser)
                    .WithMany(p => p.Timetables)
                    .HasForeignKey(d => d.EndUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__timetable__end_u__503BEA1C");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
