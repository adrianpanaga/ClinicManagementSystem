// Location: ClinicManagement.Data\Context/ClinicManagementDbContext.cs
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ClinicManagement.Data.Models; // Ensure all your custom models are in this namespace

namespace ClinicManagement.Data.Context
{
    public partial class ClinicManagementDbContext : IdentityDbContext<
        User,
        Role,
        int,
        IdentityUserClaim<int>,
        UserRole, // Your custom UserRole
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
    {
        public ClinicManagementDbContext(DbContextOptions<ClinicManagementDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; } = null!;
        public virtual DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public virtual DbSet<Patient> Patients { get; set; } = null!;
        public virtual DbSet<VerificationCode> VerificationCodes { get; set; } = null!;
        public new virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<StaffDetail> StaffDetails { get; set; } = null!;
        public new virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Vendor> Vendors { get; set; } = null!;
        public virtual DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public virtual DbSet<ItemBatch> ItemBatches { get; set; } = null!;
        public virtual DbSet<StockTransaction> StockTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // CRITICAL: This must be called first

            // --- Configure Identity Tables to use your custom names ---
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            // --- Explicitly configure the IdentityUserRole<int> (UserRoles) entity ---
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired();
                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .IsRequired();
                entity.ToTable("UserRoles");
            });

            // --- Configure User's custom properties ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                // IsActive is a simple bool, no special fluent API needed unless you have defaults/conversions.
            });

            // --- Configure Role's custom properties and seeding ---
            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
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

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC346E8240D4B");
                entity.Property(e => e.PatientId).HasColumnName("PatientID");

                // Keep explicit column name mapping for UserId, but it's just a regular column now
                entity.Property(e => e.UserId).HasColumnName("UserId");

                // Your existing query filter
                entity.HasQueryFilter(p => !p.IsDeleted);

                // Add unique indexes back (confirm these are present)
                entity.HasIndex(e => e.ContactNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // --- CRITICAL FIX: Add this line to explicitly make UserId index non-unique ---
                entity.HasIndex(e => e.UserId); // This explicitly creates a non-unique index on UserId

                // ... (all other property mappings for Patient like FirstName, LastName, etc. should be here) ...
                // ... (any other relationships from Patient to Appointment, MedicalRecord etc. that DON'T involve User directly) ...
            });


            // --- StaffDetail entity configuration (confirming existing setup) ---
            modelBuilder.Entity<StaffDetail>(entity =>
            {
                entity.HasKey(e => e.StaffId).HasName("PK__StaffDet__96D4AAF741B20436");
                entity.Property(e => e.StaffId).HasColumnName("StaffID");
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.JobTitle).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.MiddleName).HasMaxLength(50);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasColumnName("UserID"); // Ensure this maps to DB column
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                // Relationship StaffDetail to User (staff login)
                entity.HasOne(sd => sd.User)
                    .WithMany(u => u.StaffDetails) // This assumes 'User' has `ICollection<StaffDetail> StaffDetails`
                    .HasForeignKey(sd => sd.UserId)
                    .HasPrincipalKey(u => u.Id)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_StaffDetails_Users_UserId_New");
            });

            // --- Appointment entity configuration (confirming existing setup) ---
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCCC56AF500B");
                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
                entity.Property(e => e.AppointmentDateTime).HasColumnType("datetime");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.DoctorId).HasColumnName("DoctorID");
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.PatientId).HasColumnName("PatientID");
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
                entity.Property(e => e.Status).HasMaxLength(50).IsUnicode(false).HasDefaultValueSql("('Scheduled')");

                entity.HasOne(d => d.Service).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appointme__Servi__5DCAEF64");

                entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.DoctorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Appointme__Docto__5CD626F7");

                entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PatientId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Appointme__Patie__5BE2A6F2");
            });

            // --- Medical Record entity configuration (confirming existing setup) ---
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId).HasName("PK__MedicalR__FBDF78C62A86B91A");
                entity.Property(e => e.RecordId).HasColumnName("RecordID");
                entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
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
                    .HasConstraintName("FK__MedicalRe__Servi__XXXXXX"); // Placeholder constraint name
            });

            // --- Service entity configuration (confirming existing setup) ---
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId).HasName("PK__Services__C51BB0EA0B0C46B1");
                entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ServiceName).HasMaxLength(100);
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            // --- Vendor entity configuration (confirming existing setup) ---
            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.VendorId).HasName("PK__Vendors__FC8653AD1D3DE96A");
                entity.Property(e => e.VendorId).HasColumnName("VendorID");
                entity.Property(e => e.VendorName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.ContactNumber).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            });

            // --- InventoryItem entity configuration (confirming existing setup) ---
            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasKey(e => e.ItemId).HasName("PK__Inventory__7BF7D5B66736C561");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.ItemName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.UnitOfMeasure).HasMaxLength(50).IsRequired();
                entity.Property(e => e.VendorId).HasColumnName("VendorID");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(d => d.Vendor).WithMany(p => p.InventoryItems)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__Vendo__6EF57B66");
            });

            // --- ItemBatch entity configuration (confirming existing setup) ---
            modelBuilder.Entity<ItemBatch>(entity =>
            {
                entity.HasKey(e => e.BatchId).HasName("PK__ItemBatc__B28D3F372095039A");
                entity.Property(e => e.BatchId).HasColumnName("BatchID");
                entity.Property(e => e.ItemId).HasColumnName("ItemID");
                entity.Property(e => e.BatchNumber).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.ExpirationDate).HasColumnType("date");
                entity.Property(e => e.CostPerUnit).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Item).WithMany(p => p.ItemBatches)
                    .HasForeignKey(d => d.ItemId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__ItemBatch__ItemI__72C60C4A");
            });

            // --- StockTransaction entity configuration (confirming existing setup) ---
            modelBuilder.Entity<StockTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId).HasName("PK__StockTra__55433B6A7F2D065C");
                entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
                entity.Property(e => e.BatchId).HasColumnName("BatchID");
                entity.Property(e => e.TransactionType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(d => d.Batch).WithMany(p => p.StockTransactions)
                    .HasForeignKey(d => d.BatchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StockTran__Batch__75A278FBC");
            });


            // VerificationCode configuration (keep as is)
            modelBuilder.Entity<VerificationCode>()
                 .HasQueryFilter(vc => vc.Patient != null && !vc.Patient.IsDeleted)
                .HasOne(vc => vc.Patient)
                .WithMany()
                .HasForeignKey(vc => vc.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}