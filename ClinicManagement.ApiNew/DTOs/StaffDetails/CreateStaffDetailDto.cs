using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.ApiNew.DTOs.StaffDetails
{
    /// <summary>
    /// DTO for creating a new StaffDetail entry.
    /// </summary>
    public class CreateStaffDetailDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserId { get; set; } // Must link to an existing User

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters.")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(50, ErrorMessage = "Job title cannot exceed 50 characters.")]
        public string JobTitle { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters.")]
        public string? Specialization { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [StringLength(20, ErrorMessage = "Contact number cannot exceed 20 characters.")]
        public string ContactNumber { get; set; } = string.Empty;
    }
}