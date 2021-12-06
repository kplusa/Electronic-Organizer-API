using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

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
            modelBuilder.HasAnnotation("Relational:Collation", "Polish_CI_AS");

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
                    .HasConstraintName("FK__day_of_ti__day_i__00200768");

                entity.HasOne(d => d.Timetable)
                    .WithMany(p => p.DayOfTimetables)
                    .HasForeignKey(d => d.TimetableId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__day_of_ti__timet__01142BA1");
            });

            modelBuilder.Entity<EndUser>(entity =>
            {
                entity.ToTable("end_user");

                entity.HasIndex(e => e.Email, "UQ__end_user__AB6E6164E77C173A")
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
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("hashed_password");

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("salt");

                entity.HasOne(d => d.EndUser)
                    .WithMany(p => p.EndUserSecurities)
                    .HasForeignKey(d => d.EndUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__end_user___end_u__787EE5A0");
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
                    .HasConstraintName("FK__event__day_of_ti__06CD04F7");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK__event__service_i__07C12930");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("service");

                entity.HasIndex(e => e.Name, "UQ__service__72E12F1B2D6E1030")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.EstimatedTime).HasColumnName("estimated_time");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("name");
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
                    .HasConstraintName("FK__timetable__end_u__7B5B524B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
