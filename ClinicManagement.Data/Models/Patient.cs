using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For data annotations
using System.ComponentModel.DataAnnotations.Schema; // For column mapping if needed

namespace ClinicManagement.Data.Models
{
    public partial class Patient
    {
        public Patient()
        {
            Appointments = new HashSet<Appointment>();
            MedicalRecords = new HashSet<MedicalRecord>();
        }

        [Key]
        [Column("PatientID")]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        // Changed to DateOnly to match DTOs and ensure consistency
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = null!; // e.g., "Male", "Female", "Other"

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(5)]
        public string? BloodType { get; set; } // e.g., "A+", "O-", etc.

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactNumber { get; set; }

        [StringLength(255)]
        public string? PhotoUrl { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        // --- NEW/UPDATED: Link to User model ---
        // Marking as nullable (int?) to allow patients without an immediate linked user account
        // The foreign key mapping will be in ClinicContext.
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        // --- END NEW/UPDATED ---

        [InverseProperty("Patient")]
        public virtual ICollection<Appointment> Appointments { get; set; }
        [InverseProperty("Patient")]
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
