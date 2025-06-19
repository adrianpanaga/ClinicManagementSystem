// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.Data\Models\StaffDetail.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicManagement.Data.Models
{
    public partial class StaffDetail
    {
        public StaffDetail()
        {
            Appointments = new HashSet<Appointment>();
            MedicalRecords = new HashSet<MedicalRecord>();
        }

        [Key]
        public int StaffId { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(50)]
        public string? JobTitle { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        public int? UserId { get; set; } // Foreign Key to User (IdentityUser)

        // --- NEW PROPERTY FOR SOFT DELETE ---
        public bool IsDeleted { get; set; } = false; // Default to false (not deleted)

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; } // Mark as nullable

        [InverseProperty("Doctor")]
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}
