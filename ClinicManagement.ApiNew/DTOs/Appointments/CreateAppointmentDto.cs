// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.ApiNew\DTOs\Appointments\CreateAppointmentDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.ApiNew.DTOs.Appointments
{
    // DTO for creating a new Appointment (Write operations - POST)
    public class CreateAppointmentDto
    {
        public int? PatientId { get; set; } // Can be nullable if appointment doesn't require patient initially

        [Required(ErrorMessage = "Doctor ID is required.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Service ID is required.")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Appointment Date/Time is required.")]
        public DateTime AppointmentDateTime { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string? Status { get; set; } // Made nullable to match model, but [Required] ensures value on creation

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
