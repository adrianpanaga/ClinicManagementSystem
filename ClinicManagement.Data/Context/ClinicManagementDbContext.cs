// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.Data\Context/ClinicManagementDbContext.cs

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ClinicManagement.Data.Models;

namespace ClinicManagement.Data.Context
{
    public partial class ClinicManagementDbContext : IdentityDbContext<User, Role, int>
    {
        public ClinicManagementDbContext(DbContextOptions<ClinicManagementDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public new virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<StaffDetail> StaffDetails { get; set; } = null!;
        public new virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuration for Identity Tables ---
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");

            // --- Medical Record Soft Delete Global Query Filter ---
            modelBuilder.Entity<MedicalRecord>().HasQueryFilter(mr => !mr.IsDeleted);

            // --- Patient Soft Delete Global Query Filter ---
            modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);

            // --- User entity configuration ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.RoleId).HasColumnName("RoleID");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Users__RoleID__45F365D3_New");
            });

            // --- Role entity configuration ---
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasData(
                    new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 2, Name = "Receptionist", NormalizedName = "RECEPTIONIST", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 3, Name = "HR", NormalizedName = "HR", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 4, Name = "Doctor", NormalizedName = "DOCTOR", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 5, Name = "Nurse", NormalizedName = "NURSE", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 6, Name = "Patient", NormalizedName = "PATIENT", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 7, Name = "InventoryManager", NormalizedName = "INVENTORYMANAGER", CreatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc) }
                );
            });

            // --- Update Appointment entity configuration for nullable PatientId ---
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCCC56AF500B");
                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.PatientId).HasColumnName("PatientID"); // Still maps to the same column
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Scheduled')");

                entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Servi__5DCAEF64");

                entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Docto__5CD626F7");

                // Update this foreign key relationship to be optional
                entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .IsRequired(false) // Make this relationship optional
                    .OnDelete(DeleteBehavior.SetNull) // Set FK to NULL if Patient is deleted
                    .HasConstraintName("FK__Appointme__Patie__5BE2A6F2");
            });

            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C62A86B91A");
                entity.Property(e => e.RecordId).HasColumnName("RecordID");
                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Diagnosis).HasMaxLength(500);
                entity.Property(e => e.PatientId).HasColumnName("PatientID");
                entity.Property(e => e.Prescription).HasMaxLength(1000);
                entity.Property(e => e.Treatment).HasMaxLength(1000);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
                entity.Property(e => e.StaffId).HasColumnName("StaffID");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);


                entity.HasOne(d => d.Appointment).WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MedicalRe__Appoi__60A75C0F");

                entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MedicalRe__Patie__5FB337D6");

                entity.HasOne(d => d.Staff).WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MedicalRecords_StaffDetails_StaffID");

                entity.HasOne(d => d.Service).WithMany(p => p.MedicalRecords)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__MedicalRe__Servi__XXXXXX");

            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346E8240D4B");
                entity.HasIndex(e => e.ContactNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.PatientId).HasColumnName("PatientID");
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.BloodType).HasMaxLength(5);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DateOfBirth).HasColumnType("date");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
                entity.Property(e => e.EmergencyContactNumber).HasMaxLength(20);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.MiddleName).HasMaxLength(50);
                entity.Property(e => e.PhotoUrl).HasMaxLength(255);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);


                entity.HasOne(d => d.User)
                    .WithOne(p => p.Patient!)
                    .HasForeignKey<Patient>(d => d.UserId)
                    .HasPrincipalKey<User>(u => u.Id)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Patients_Users_UserId_New");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA0B0C46B1");
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ServiceName).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<StaffDetail>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__StaffDet__96D4AAF741B20436");
                entity.Property(e => e.StaffId).HasColumnName("StaffID");
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.JobTitle).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.MiddleName).HasMaxLength(50);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(u => u.StaffDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasPrincipalKey(u => u.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDetails_Users_UserId_New");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
