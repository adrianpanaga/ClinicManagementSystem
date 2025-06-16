namespace ClinicManagement.ApiNew.DTOs.Appointments
{
    /// <summary>
    /// DTO for creating a new appointment.
    /// Uses AppointmentDateTime and DurationMinutes as per the model.
    /// </summary>
    public class CreateAppointmentDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDateTime { get; set; } // Combined date and time for start
        public int DurationMinutes { get; set; }          // Duration of the appointment
        public string? Notes { get; set; }
    }
}
