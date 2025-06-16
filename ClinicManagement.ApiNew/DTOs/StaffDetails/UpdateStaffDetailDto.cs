using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.ApiNew.DTOs.StaffDetails
{
    /// <summary>
    /// DTO for updating an existing StaffDetail entry.
    /// All properties are nullable to allow for partial updates.
    /// </summary>
    public class UpdateStaffDetailDto
    {
        // UserId can be updated, but care must be taken (e.g., only by Admin)
        // If 0 is sent, it implies unlinking (if StaffDetail.UserId was nullable),
        // but since it's not nullable in your model, a valid UserId is always expected.
        // We'll address this in the controller if you want to allow changing linked user.
        public int? UserId { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
        public string? MiddleName { get; set; }

        [StringLength(50, ErrorMessage = "Job title cannot exceed 50 characters.")]
        public string? JobTitle { get; set; }

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string? Specialization { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "Contact number cannot exceed 20 characters.")]
        public string? ContactNumber { get; set; }
    }
}