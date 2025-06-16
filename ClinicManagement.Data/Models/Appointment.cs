using System;
using System.Collections.Generic;

namespace ClinicManagement.Data.Models
{
    public partial class Appointment
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDateTime { get; set; } // Represents the start date and time
        public int DurationMinutes { get; set; }          // Duration of the appointment in minutes
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual StaffDetail Doctor { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
