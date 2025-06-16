// Location: ClinicManagement.Data/Models/StaffDetail.cs
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
        }

        [Key]
        [Column("StaffID")]
        public int StaffId { get; set; }

        [Column("UserID")]
        public int UserId { get; set; } // Foreign key to User

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string JobTitle { get; set; } = null!;

        [StringLength(100)]
        public string? Specialization { get; set; }

        [Required] // Added [Required] for Email
        [StringLength(100)] // Added StringLength for Email
        [EmailAddress] // Added EmailAddress validation
        public string Email { get; set; } = null!; // Added Email property

        [Required] // Added [Required] for ContactNumber
        [StringLength(20)] // Added StringLength for ContactNumber
        public string ContactNumber { get; set; } = null!; // Added ContactNumber property

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("StaffDetails")]
        public virtual User User { get; set; } = null!;

        [InverseProperty("Doctor")]
        public virtual ICollection<Appointment> Appointments { get; set; }

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
