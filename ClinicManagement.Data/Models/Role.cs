// Location: ClinicManagement.Data/Models/Role.cs
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity; // <--- ADD THIS USING DIRECTIVE

namespace ClinicManagement.Data.Models;

// Your Role model must now inherit from IdentityRole<int>
public partial class Role : IdentityRole<int> // <--- CRITICAL CHANGE HERE
{
    public Role()
    {
        // IdentityRole handles collections for roles/users internally (e.g., AspNetUserRoles join table).
        // If your User model uses a direct RoleId (which yours does), then this Users collection is still relevant.
        Users = new List<User>();
    }

    // REMOVED PROPERTIES:
    // You NO LONGER need these properties here, as they are provided by IdentityRole<int>:
    // - RoleId (Use 'Id' from IdentityRole<int>)
    // - RoleName (Use 'Name' from IdentityRole)

    // For example, if your old Role.cs had:
    // public int RoleId { get; set; }
    // public string RoleName { get; set; } = null!;
    // YOU MUST DELETE THESE LINES.


    // KEEP THESE CUSTOM PROPERTIES:
    // (These are additional properties specific to your application)
    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // KEEP THIS NAVIGATION PROPERTY:
    // (This is relevant because your User model has a direct RoleId foreign key)
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}