using System;
using System.Text.Json.Serialization; // For DateOnly JSON serialization

namespace ClinicManagement.ApiNew.DTOs.Patients
{
    /// <summary>
    /// DTO for returning comprehensive patient details in API responses.
    /// Includes basic linked user info if available.
    /// </summary>
    public class PatientDetailsDto
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))] // Ensure DateOnly is serialized correctly
        public DateOnly DateOfBirth { get; set; } // Changed from DateTime to DateOnly
        public string Gender { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? BloodType { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; } // This property is DateTime in your model

        // Linked User Account Details (if available)
        public int? UserId { get; set; }
        /* REMOVED: Username, UserEmail, UserRole as they are sensitive details.
        public string? Username { get; set; }
        public string? UserEmail { get; set; } // Redundant with patient email but good for clarity
        public string? UserRole { get; set; }
        */
    }
}