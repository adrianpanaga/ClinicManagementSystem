using System;
using System.ComponentModel.DataAnnotations; // For data annotations
using System.Text.Json.Serialization; // For DateOnly JSON serialization

namespace ClinicManagement.ApiNew.DTOs.Patients
{
    /// <summary>
    /// DTO for updating an existing patient's profile.
    /// Allows partial updates of fields.
    /// </summary>
    public class UpdatePatientDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [JsonConverter(typeof(DateOnlyJsonConverter))] // Ensure DateOnly is serialized correctly
        public DateOnly? DateOfBirth { get; set; } // Changed from DateTime? to DateOnly?

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(5)]
        public string? BloodType { get; set; }

        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(20)]
        public string? EmergencyContactNumber { get; set; }

        [MaxLength(255)]
        public string? PhotoUrl { get; set; }

        // Allow updating the linked UserId, e.g., if linking an existing patient to a new user account
        public int? UserId { get; set; }
    }
}