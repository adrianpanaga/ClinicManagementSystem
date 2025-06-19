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
        // Add DbSets for new Inventory Management models
        public virtual DbSet<Vendor> Vendors { get; set; } = null!;
        public virtual DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public virtual DbSet<ItemBatch> ItemBatches { get; set; } = null!;
        public virtual DbSet<StockTransaction> StockTransactions { get; set; } = null!;


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

            // --- Soft Delete Global Query Filters ---
            modelBuilder.Entity<MedicalRecord>().HasQueryFilter(mr => !mr.IsDeleted);
            modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<StaffDetail>().HasQueryFilter(sd => !sd.IsDeleted); // StaffDetail soft delete filter
            modelBuilder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted); // Vendor soft delete filter
            modelBuilder.Entity<InventoryItem>().HasQueryFilter(i => !i.IsDeleted); // InventoryItem soft delete filter


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

            // --- Appointment entity configuration (with nullable PatientId and DoctorId) ---
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCCC56AF500B");
                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DoctorId).HasColumnName("DoctorID"); // Mapped to nullable column
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.PatientId).HasColumnName("PatientID"); // Mapped to nullable column
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Scheduled')");

                entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Servi__5DCAEF64");

                // Updated: Relationship to Doctor (StaffDetail) is now optional for soft delete
                entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.DoctorId)
                    .IsRequired(false) // Make this relationship optional
                    .OnDelete(DeleteBehavior.SetNull) // Set FK to NULL if StaffDetail (Doctor) is soft-deleted
                    .HasConstraintName("FK__Appointme__Docto__5CD626F7");

                // Patient relationship is already optional from previous fix
                entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Appointme__Patie__5BE2A6F2");
            });

            // --- Medical Record entity configuration (with soft delete) ---
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

            // --- Patient entity configuration (with soft delete) ---
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

            // --- Service entity configuration ---
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

            // --- StaffDetail entity configuration (with soft delete) ---
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
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(d => d.User)
                    .WithMany(u => u.StaffDetails)
                    .HasForeignKey(d => d.UserId)
                    .HasPrincipalKey(u => u.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StaffDetails_Users_UserId_New");
            });

            // --- Vendor entity configuration (with soft delete) ---
            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.VendorId).HasName("PK__Vendors__FC8653AD1D3DE96A");
                entity.Property(e => e.VendorId).HasColumnName("VendorID");
                entity.Property(e => e.VendorName)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            // --- InventoryItem entity configuration (with soft delete) ---
            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasKey(e => e.ItemId).HasName("PK__Inventory__7BF7D5B66736C561");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.ItemName)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.UnitOfMeasure) // Added: Configuration for Unit property
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.VendorId).HasColumnName("VendorID"); // Added: Configuration for VendorId
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(d => d.Vendor).WithMany(p => p.InventoryItems)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__Vendo__6EF57B66");
            });

            // --- ItemBatch entity configuration ---
            modelBuilder.Entity<ItemBatch>(entity =>
            {
                entity.HasKey(e => e.BatchId).HasName("PK__ItemBatc__B28D3F372095039A");
                entity.Property(e => e.BatchId).HasColumnName("BatchID");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.BatchNumber)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.ExpirationDate).HasColumnType("date"); // Added: Configuration for ExpiryDate
                entity.Property(e => e.CostPerUnit).HasColumnType("decimal(18, 2)"); // Added: Configuration for PurchasePrice
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)"); // Added: Configuration for SalePrice
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Item).WithMany(p => p.ItemBatches)
                    .HasForeignKey(d => d.ItemId)
                    .IsRequired(false) // ItemId can be null if an item is soft-deleted
                    .OnDelete(DeleteBehavior.SetNull) // Set FK to NULL if InventoryItem is deleted
                    .HasConstraintName("FK__ItemBatch__ItemI__72C60C4A");
            });

            // --- StockTransaction entity configuration ---
            modelBuilder.Entity<StockTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId).HasName("PK__StockTra__55433B6A7F2D065C");
                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
                entity.Property(e => e.BatchId).HasColumnName("BatchID");
                entity.Property(e => e.TransactionType)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.TransactionDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(d => d.Batch).WithMany(p => p.StockTransactions)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StockTran__Batch__75A278FBC");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
