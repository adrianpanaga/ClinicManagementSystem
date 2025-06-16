// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.ApiNew\DTOs\MedicalRecords\MedicalRecordsDto.cs

using System;
using ClinicManagement.ApiNew.DTOs.Appointments; // Using existing Appointment DTO
using ClinicManagement.ApiNew.DTOs.Patients;    // Using existing Patient DTO
using ClinicManagement.ApiNew.DTOs.StaffDetails; // Using existing StaffDetail DTO
// Add using for your Services DTO if it exists, e.g.:
// using ClinicManagement.ApiNew.DTOs.Services; 

namespace ClinicManagement.ApiNew.DTOs.MedicalRecords
{
    // DTO for retrieving Medical Record data (Read operations)
    public class MedicalRecordsDto
    {
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; }
        public int StaffId { get; set; } // Doctor who created the record
        public int? ServiceId { get; set; } // Service associated with this record
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Prescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Include DTOs for related entities for a complete response
        // Note: These will map from your existing DTO types
        public PatientDetailsDto? Patient { get; set; } // Assuming your Patient DTO is PatientDetailsDto
        public StaffDetailDto? Staff { get; set; }
        public AppointmentDto? Appointment { get; set; }
        // public ServiceDto? Service { get; set; } // Uncomment if you have a ServiceDto, otherwise remove
    }
}
