﻿namespace ClinicManagement.ApiNew.DTOs.Users
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
