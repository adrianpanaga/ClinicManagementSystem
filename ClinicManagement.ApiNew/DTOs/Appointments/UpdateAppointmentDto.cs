namespace ClinicManagement.ApiNew.DTOs.Appointments
{
    /// <summary>
    /// DTO for updating an existing appointment.
    /// Allows partial updates.
    /// </summary>
    public class UpdateAppointmentDto
    {
        public int? DoctorId { get; set; }
        public int? ServiceId { get; set; }
        public DateTime? AppointmentDateTime { get; set; } // Combined date and time for start
        public int? DurationMinutes { get; set; }          // Duration of the appointment
        public string? Status { get; set; } // E.g., "Scheduled", "Confirmed", "Completed", "Cancelled"
        public string? Notes { get; set; }
    }
}
