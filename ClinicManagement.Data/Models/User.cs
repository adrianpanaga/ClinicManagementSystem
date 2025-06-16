// Location: ClinicManagement.Data/Models/User.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity; // <--- ADD THIS USING DIRECTIVE

namespace ClinicManagement.Data.Models
{
    // Your User model must now inherit from IdentityUser<int>
    public partial class User : IdentityUser<int> // <--- CRITICAL CHANGE HERE
    {
        public User()
        {
            // IdentityUser handles collections for roles/claims etc. automatically.
            // You only need to initialize your custom collections.
            StaffDetails = new HashSet<StaffDetail>();
        }

        // REMOVED PROPERTIES:
        // You NO LONGER need these properties here, as they are provided by IdentityUser<int>:
        // - UserId (Use 'Id' from IdentityUser<int>)
        // - Username (Use 'UserName' from IdentityUser)
        // - Email (Use 'Email' from IdentityUser)
        // - PasswordHash (Use 'PasswordHash' from IdentityUser)

        // For example, if your old User.cs had:
        // [Key]
        // [Column("UserID")]
        // public int UserId { get; set; }
        // public string Username { get; set; } = null!;
        // public string Email { get; set; } = null!;
        // public string PasswordHash { get; set; } = null!;
        // YOU MUST DELETE THESE LINES.

        // KEEP THESE CUSTOM PROPERTIES:
        // (These are additional properties specific to your application that IdentityUser does not provide)
        [Column("RoleId")] // This maps your custom RoleId foreign key in your database
        public int RoleId { get; set; }

        public bool IsActive { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        // KEEP THESE NAVIGATION PROPERTIES:
        // (These define relationships to other entities in your database)
        [ForeignKey("RoleId")]
        [InverseProperty("Users")]
        public virtual Role Role { get; set; } = null!; // Link to your custom Role model

        [InverseProperty("User")] // One-to-one relationship with Patient
        public virtual Patient? Patient { get; set; } // Nullable for users who are not patients

        [InverseProperty("User")] // One-to-many relationship with StaffDetail
        public virtual ICollection<StaffDetail> StaffDetails { get; set; }
    }
}