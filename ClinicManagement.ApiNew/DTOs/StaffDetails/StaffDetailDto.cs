using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.ApiNew.DTOs.StaffDetails
{
    /// <summary>
    /// DTO for returning StaffDetail information in API responses.
    /// </summary>
    public class StaffDetailDto
    {
        public int StaffId { get; set; }
        public int UserId { get; set; } // The ID of the linked user

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}