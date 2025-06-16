namespace ClinicManagement.ApiNew.DTOs.Appointments
{
    /// <summary>
    /// DTO for returning appointment details in API responses.
    /// Includes related patient, doctor, and service info for convenience.
    /// EndDateTime is calculated.
    /// </summary>
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDateTime { get; set; } // The start date and time
        public int DurationMinutes { get; set; }          // Duration of the appointment
        public DateTime EndDateTime { get; set; }         // Calculated End Time for convenience
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Patient details
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public string PatientContactNumber { get; set; } = string.Empty;

        // Doctor details
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public string DoctorSpecialization { get; set; } = string.Empty;

        // Service details
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal ServicePrice { get; set; }
    }
}
