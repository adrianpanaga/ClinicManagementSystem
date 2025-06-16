using System;
using System.ComponentModel.DataAnnotations; // For data annotations
using System.Text.Json.Serialization; // For DateOnly JSON serialization

namespace ClinicManagement.ApiNew.DTOs.Patients
{
    /// <summary>
    /// DTO for creating a new patient profile.
    /// Note: This DTO is for creating the patient's demographic and contact info.
    /// Linking to a User account (for login) is a separate step/process, or can be done during registration.
    /// The UserId will be passed directly if a new User is created with the patient.
    /// </summary>
    public class CreatePatientDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [JsonConverter(typeof(DateOnlyJsonConverter))] // Ensure DateOnly is serialized correctly
        public DateOnly DateOfBirth { get; set; } // Changed from DateTime to DateOnly

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty; // E.g., "Male", "Female", "Other"

        [Required]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(5)]
        public string? BloodType { get; set; } // E.g., "A+", "O-", etc.

        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(20)]
        public string? EmergencyContactNumber { get; set; }

        [MaxLength(255)]
        public string? PhotoUrl { get; set; } // URL to patient's photo

        // If creating a patient and linking them to an *existing* user account immediately,
        // you might include UserId here. For initial patient creation, it might be null
        // if the patient doesn't have an online account yet.
        public int? UserId { get; set; }
    }
}