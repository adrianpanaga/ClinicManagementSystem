// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.Data\Models\Appointment.cs

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Data.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            MedicalRecords = new HashSet<MedicalRecord>();
        }

        [Key]
        public int AppointmentId { get; set; }

        // --- IMPORTANT CHANGE: Make PatientId nullable ---
        public int? PatientId { get; set; }

        public int DoctorId { get; set; }

        public int ServiceId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime AppointmentDateTime { get; set; }

        [StringLength(50)]
        [Unicode(false)]
        public string? Status { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        // --- IMPORTANT CHANGE: Make Patient navigation property nullable ---
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; } // Now nullable

        [ForeignKey("DoctorId")]
        public virtual StaffDetail? Doctor { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
