// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.ApiNew\DTOs\Appointments\AppointmentDto.cs

using System;
using ClinicManagement.ApiNew.DTOs.Patients;     // For PatientDetailsDto
using ClinicManagement.ApiNew.DTOs.StaffDetails; // For StaffDetailDto (Doctor)
using ClinicManagement.ApiNew.DTOs.Services;    // For ServiceDto

namespace ClinicManagement.ApiNew.DTOs.Appointments
{
    // DTO for retrieving Appointment data (Read operations)
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int? PatientId { get; set; } // Nullable as per model
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // --- ADDED NAVIGATION PROPERTIES TO DTO ---
        public PatientDetailsDto? Patient { get; set; } // Matches PatientDetailsDto
        public StaffDetailDto? Doctor { get; set; }     // For the associated Doctor (StaffDetail)
        public ServiceDto? Service { get; set; }        // For the associated Service
    }
}
