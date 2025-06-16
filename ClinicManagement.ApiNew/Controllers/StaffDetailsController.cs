using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data.Context;
using ClinicManagement.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClinicManagement.ApiNew.DTOs.StaffDetails;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity; // Added for UserManager

namespace ClinicManagement.ApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints in this controller now require authentication by default
    public class StaffDetailsController : ControllerBase
    {
        private readonly ClinicManagementDbContext _context;
        private readonly ILogger<StaffDetailsController> _logger;
        private readonly UserManager<User> _userManager; // Injected UserManager

        public StaffDetailsController(ClinicManagementDbContext context,
                                      ILogger<StaffDetailsController> logger,
                                      UserManager<User> userManager) // Added UserManager to constructor
        {
            _context = context;
            _logger = logger;
            _userManager = userManager; // Assign injected UserManager
        }

        /// <summary>
        /// Gets a list of all staff profiles. Requires 'Admin' or 'HR' role.
        /// </summary>
        /// <returns>A list of StaffDetailDto objects.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,HR,Doctor,Receptionist,Nurse")] // Define roles that can view all staff
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<StaffDetailDto>>> GetAllStaffDetails()
        {
            _logger.LogInformation("Attempting to retrieve all staff details by {User}", User.Identity?.Name);

            if (_context.StaffDetails == null)
            {
                _logger.LogWarning("StaffDetails DbSet is null in GetAllStaffDetails.");
                return NotFound("Staff details entity set not available.");
            }

            var staff = await _context.StaffDetails
                                      .Include(s => s.User)
                                      .Select(s => new StaffDetailDto
                                      {
                                          StaffId = s.StaffId,
                                          UserId = s.UserId,
                                          FirstName = s.FirstName,
                                          LastName = s.LastName,
                                          MiddleName = s.MiddleName,
                                          JobTitle = s.JobTitle,
                                          Specialization = s.Specialization,
                                          Email = s.Email,
                                          ContactNumber = s.ContactNumber,
                                          CreatedAt = s.CreatedAt,
                                          UpdatedAt = s.UpdatedAt
                                      })
                                      .ToListAsync();

            _logger.LogInformation("Retrieved {Count} staff details.", staff.Count);
            return Ok(staff);
        }

        /// <summary>
        /// Custom endpoint to retrieve only staff members whose JobTitle is "Doctor". Requires 'Admin', 'HR', 'Receptionist', 'Nurse' or 'Doctor' role.
        /// </summary>
        /// <returns>A list of StaffDetailDto objects for doctors.</returns>
        [HttpGet("doctors")]
        [Authorize(Roles = "Admin,HR,Receptionist,Nurse,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<StaffDetailDto>>> GetDoctors()
        {
            _logger.LogInformation("Attempting to retrieve all doctors by {User}", User.Identity?.Name);

            if (_context.StaffDetails == null)
            {
                _logger.LogWarning("StaffDetails DbSet is null in GetDoctors.");
                return NotFound("Staff details entity set not available.");
            }

            var doctors = await _context.StaffDetails
                                        .Include(s => s.User)
                                        .Where(s => s.JobTitle.ToLower() == "doctor")
                                        .Select(s => new StaffDetailDto
                                        {
                                            StaffId = s.StaffId,
                                            UserId = s.UserId,
                                            FirstName = s.FirstName,
                                            LastName = s.LastName,
                                            MiddleName = s.MiddleName,
                                            JobTitle = s.JobTitle,
                                            Specialization = s.Specialization,
                                            Email = s.Email,
                                            ContactNumber = s.ContactNumber,
                                            CreatedAt = s.CreatedAt,
                                            UpdatedAt = s.UpdatedAt
                                        })
                                        .ToListAsync();

            _logger.LogInformation("Retrieved {Count} doctors.", doctors.Count);
            return Ok(doctors);
        }

        /// <summary>
        /// Gets details for a specific staff member by their StaffId. Accessible by 'Admin', 'HR', or the staff member themselves.
        /// </summary>
        /// <param name="id">The ID of the staff member.</param>
        /// <returns>The StaffDetailDto object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StaffDetailDto>> GetStaffDetail(int id)
        {
            _logger.LogInformation("Attempting to retrieve staff detail {StaffId} by {User}", id, User.Identity?.Name);

            if (_context.StaffDetails == null)
            {
                _logger.LogWarning("StaffDetails DbSet is null in GetStaffDetail.");
                return NotFound("Staff details entity set not available.");
            }

            var staffDetail = await _context.StaffDetails
                                            .Include(s => s.User)
                                                .ThenInclude(u => u.Role)
                                            .FirstOrDefaultAsync(s => s.StaffId == id);

            if (staffDetail == null)
            {
                _logger.LogWarning("Staff with ID {StaffId} not found.", id);
                return NotFound($"Staff with ID {id} not found.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAuthorizedStaffRole = User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Receptionist") || User.IsInRole("Nurse") || User.IsInRole("Doctor");
            bool isSelf = int.TryParse(userIdClaim, out int currentUserId) && staffDetail.UserId == currentUserId;

            if (!isAuthorizedStaffRole && !isSelf)
            {
                _logger.LogWarning("User {User} attempted to access staff detail {StaffId} without proper authorization.", User.Identity?.Name, id);
                return Forbid("You are not authorized to view this staff member's details.");
            }

            var staffDetailDto = new StaffDetailDto
            {
                StaffId = staffDetail.StaffId,
                UserId = staffDetail.UserId,
                FirstName = staffDetail.FirstName,
                LastName = staffDetail.LastName,
                MiddleName = staffDetail.MiddleName,
                JobTitle = staffDetail.JobTitle,
                Specialization = staffDetail.Specialization,
                Email = staffDetail.Email,
                ContactNumber = staffDetail.ContactNumber,
                CreatedAt = staffDetail.CreatedAt,
                UpdatedAt = staffDetail.UpdatedAt
            };

            _logger.LogInformation("Staff detail {StaffId} retrieved successfully.", id);
            return Ok(staffDetailDto);
        }

        /// <summary>
        /// Registers a new staff profile and their associated user login account in a single request.
        /// Requires 'Admin' or 'HR' role.
        /// The JobTitle in the DTO will be used as the User's role.
        /// </summary>
        /// <param name="registerStaffDto">Combined staff and user registration details.</param>
        /// <returns>The created StaffDetailDto object.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,HR")] // Only Admin or HR can register new staff
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StaffDetailDto>> RegisterStaff([FromBody] RegisterStaffDto registerStaffDto)
        {
            _logger.LogInformation("Attempting to register new staff and user account by {User}", User.Identity?.Name);

            // Basic checks for required DbSet
            if (_context.StaffDetails == null || _context.Users == null || _context.Roles == null || _userManager == null)
            {
                _logger.LogError("Required DbSets or UserManager are null during staff registration.");
                return Problem("Server configuration error: Required services not available.");
            }

            // --- Validate and Create User Account First ---
            // Check for duplicate username
            if (await _userManager.FindByNameAsync(registerStaffDto.Username) != null)
            {
                _logger.LogWarning("Attempted staff registration with duplicate username: {Username}", registerStaffDto.Username);
                return BadRequest("Username already exists.");
            }

            // Check for duplicate email across ALL users (not just staff)
            if (await _userManager.FindByEmailAsync(registerStaffDto.Email) != null)
            {
                _logger.LogWarning("Attempted staff registration with duplicate email: {Email}", registerStaffDto.Email);
                return BadRequest("Email already registered with another user account.");
            }

            // Verify JobTitle corresponds to an existing Role
            // We assume JobTitle (e.g., "Doctor", "Receptionist") is also a valid User Role.
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == registerStaffDto.JobTitle);
            if (role == null)
            {
                _logger.LogWarning("Attempted staff registration with invalid JobTitle/Role: {JobTitle}", registerStaffDto.JobTitle);
                return BadRequest($"Invalid JobTitle. No matching user role found for '{registerStaffDto.JobTitle}'.");
            }
            // Disallow creating 'Admin' or 'HR' staff through this endpoint by non-Admins (optional, but good practice)
            if (registerStaffDto.JobTitle == "Admin" || registerStaffDto.JobTitle == "HR")
            {
                if (!User.IsInRole("Admin")) // Only an existing Admin can create other Admin/HR accounts
                {
                    _logger.LogWarning("User {User} attempted to create staff with Admin/HR role without Admin privileges.", User.Identity?.Name);
                    return Forbid("Only an Admin can create staff with 'Admin' or 'HR' roles.");
                }
            }


            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newUser = new User
                {
                    UserName = registerStaffDto.Username,
                    Email = registerStaffDto.Email,
                    EmailConfirmed = true, // For simplicity, assume confirmed immediately
                    // Set other User properties if needed (e.g., PhoneNumber)
                };

                var createUserResult = await _userManager.CreateAsync(newUser, registerStaffDto.Password);
                if (!createUserResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create user account for {Username}: {Errors}", registerStaffDto.Username, errors);
                    return BadRequest($"Failed to create user account: {errors}");
                }

                // Add the user to the specified role
                var addToRoleResult = await _userManager.AddToRoleAsync(newUser, registerStaffDto.JobTitle);
                if (!addToRoleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to assign role {Role} to user {Username}: {Errors}", registerStaffDto.JobTitle, registerStaffDto.Username, errors);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to assign role: {errors}");
                }

                // --- Create Staff Detail ---
                var newStaffDetail = new StaffDetail
                {
                    UserId = newUser.Id, // Link to the newly created User's ID
                    FirstName = registerStaffDto.FirstName,
                    LastName = registerStaffDto.LastName,
                    MiddleName = registerStaffDto.MiddleName,
                    JobTitle = registerStaffDto.JobTitle,
                    Specialization = registerStaffDto.Specialization,
                    Email = registerStaffDto.Email, // Staff email matches user email
                    ContactNumber = registerStaffDto.ContactNumber,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.StaffDetails.Add(newStaffDetail);
                await _context.SaveChangesAsync(); // Save StaffDetail

                await transaction.CommitAsync(); // Commit the transaction if both succeed

                _logger.LogInformation("New staff {StaffFullName} (ID: {StaffId}) and user {Username} (ID: {UserId}, Role: {Role}) registered successfully by {PerformingUser}",
                                       $"{newStaffDetail.FirstName} {newStaffDetail.LastName}", newStaffDetail.StaffId, newUser.UserName, newUser.Id, registerStaffDto.JobTitle, User.Identity?.Name);

                var staffDetailDto = new StaffDetailDto
                {
                    StaffId = newStaffDetail.StaffId,
                    UserId = newStaffDetail.UserId,
                    FirstName = newStaffDetail.FirstName,
                    LastName = newStaffDetail.LastName,
                    MiddleName = newStaffDetail.MiddleName,
                    JobTitle = newStaffDetail.JobTitle,
                    Specialization = newStaffDetail.Specialization,
                    Email = newStaffDetail.Email,
                    ContactNumber = newStaffDetail.ContactNumber,
                    CreatedAt = newStaffDetail.CreatedAt,
                    UpdatedAt = newStaffDetail.UpdatedAt
                };

                return CreatedAtAction(nameof(GetStaffDetail), new { id = staffDetailDto.StaffId }, staffDetailDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback on any error
                _logger.LogError(ex, "An error occurred during staff and user registration for {Username}.", registerStaffDto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during staff and user registration.");
            }
        }

        /// <summary>
        /// Updates an existing staff member's details. Accessible by 'Admin', 'HR', or the staff member themselves.
        /// </summary>
        /// <param name="id">The ID of the staff member to update.</param>
        /// <param name="updateStaffDetailDto">Updated staff details.</param>
        /// <returns>No content on success.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStaffDetail(int id, [FromBody] UpdateStaffDetailDto updateStaffDetailDto)
        {
            _logger.LogInformation("Attempting to update staff detail {StaffId} by {User}", id, User.Identity?.Name);

            var staffToUpdate = await _context.StaffDetails
                                              .Include(s => s.User)
                                              .FirstOrDefaultAsync(s => s.StaffId == id);

            if (staffToUpdate == null)
            {
                _logger.LogWarning("Staff with ID {StaffId} not found for update.", id);
                return NotFound($"Staff with ID {id} not found.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Forbid();
            }

            bool isAdminOrHR = User.IsInRole("Admin") || User.IsInRole("HR");
            bool isSelf = int.TryParse(userIdClaim, out int currentUserId) && staffToUpdate.UserId == currentUserId;

            if (!isAdminOrHR && !isSelf)
            {
                _logger.LogWarning("User {User} attempted to update staff detail {StaffId} without proper authorization.", User.Identity?.Name, id);
                return Forbid("You are not authorized to update this staff member's details.");
            }

            // Special handling for UserId: Only Admin/HR should be able to change UserId
            if (updateStaffDetailDto.UserId.HasValue && staffToUpdate.UserId != updateStaffDetailDto.UserId.Value)
            {
                if (!isAdminOrHR)
                {
                    if (!isSelf || (isSelf && updateStaffDetailDto.UserId.Value != currentUserId))
                    {
                        _logger.LogWarning("User {User} attempted to change UserId for staff {StaffId} without Admin/HR role or not self-linking to own ID.", User.Identity?.Name, id);
                        return Forbid("You are not authorized to change the linked user account for this staff member.");
                    }
                }

                var newLinkedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == updateStaffDetailDto.UserId.Value);
                if (newLinkedUser == null)
                {
                    _logger.LogWarning("Attempted to link staff {StaffId} to non-existent UserId {NewUserId}.", id, updateStaffDetailDto.UserId.Value);
                    return NotFound($"User with ID {updateStaffDetailDto.UserId.Value} not found for linking.");
                }
                if (await _context.StaffDetails.AnyAsync(s => s.UserId == updateStaffDetailDto.UserId.Value && s.StaffId != id))
                {
                    _logger.LogWarning("Attempted to link staff {StaffId} to UserId {NewUserId} which is already linked to another staff.", id, updateStaffDetailDto.UserId.Value);
                    return BadRequest($"User with ID {updateStaffDetailDto.UserId.Value} is already linked to another staff member.");
                }
                staffToUpdate.UserId = updateStaffDetailDto.UserId.Value;
            }

            // Validate email unique if updated
            if (updateStaffDetailDto.Email != null && await _context.StaffDetails.AnyAsync(s => s.Email == updateStaffDetailDto.Email && s.StaffId != id))
            {
                _logger.LogWarning("Attempted to update staff {StaffId} with duplicate email: {Email}", id, updateStaffDetailDto.Email);
                return BadRequest("Email already in use by another staff member.");
            }
            // Validate contact number unique if updated
            if (updateStaffDetailDto.ContactNumber != null && await _context.StaffDetails.AnyAsync(s => s.ContactNumber == updateStaffDetailDto.ContactNumber && s.StaffId != id))
            {
                _logger.LogWarning("Attempted to update staff {StaffId} with duplicate contact number: {ContactNumber}", id, updateStaffDetailDto.ContactNumber);
                return BadRequest("Contact number already in use by another staff member.");
            }

            // Apply updates from DTO to model
            if (updateStaffDetailDto.FirstName != null) staffToUpdate.FirstName = updateStaffDetailDto.FirstName;
            if (updateStaffDetailDto.LastName != null) staffToUpdate.LastName = updateStaffDetailDto.LastName;
            if (updateStaffDetailDto.MiddleName != null) staffToUpdate.MiddleName = updateStaffDetailDto.MiddleName;
            if (updateStaffDetailDto.JobTitle != null) staffToUpdate.JobTitle = updateStaffDetailDto.JobTitle;
            if (updateStaffDetailDto.Specialization != null) staffToUpdate.Specialization = updateStaffDetailDto.Specialization;
            if (updateStaffDetailDto.Email != null) staffToUpdate.Email = updateStaffDetailDto.Email;
            if (updateStaffDetailDto.ContactNumber != null) staffToUpdate.ContactNumber = updateStaffDetailDto.ContactNumber;

            staffToUpdate.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Staff {StaffId} updated successfully by {PerformingUser}", id, User.Identity?.Name ?? "Unknown");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!StaffDetailExists(id))
                {
                    _logger.LogWarning(ex, "Concurrency error: Staff with ID {StaffId} not found after update attempt.", id);
                    return NotFound($"Staff with ID {id} not found after update attempt.");
                }
                _logger.LogError(ex, "Concurrency error updating staff {StaffId}.", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff {StaffId}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the staff member.");
            }
        }

        /// <summary>
        /// Deletes a staff profile. Requires 'Admin' or 'HR' role.
        /// This is a hard delete and should be used with caution. Consider soft-delete instead.
        /// </summary>
        /// <param name="id">The ID of the staff member to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,HR")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStaffDetail(int id)
        {
            _logger.LogInformation("Attempting to delete staff detail {StaffId} by {User}", id, User.Identity?.Name);

            if (_context.StaffDetails == null)
            {
                _logger.LogWarning("StaffDetails DbSet is null during staff deletion.");
                return NotFound("Staff details entity set not available.");
            }

            var staffToDelete = await _context.StaffDetails.FirstOrDefaultAsync(s => s.StaffId == id);

            if (staffToDelete == null)
            {
                _logger.LogWarning("Staff with ID {StaffId} not found for deletion.", id);
                return NotFound($"Staff with ID {id} not found.");
            }

            // IMPORTANT: If you delete StaffDetail, consider if the linked User account should also be deleted.
            // For now, only the StaffDetail is deleted. The User account remains.
            // If you want to delete the user as well, add:
            // var userToDelete = await _userManager.FindByIdAsync(staffToDelete.UserId.ToString());
            // if (userToDelete != null) await _userManager.DeleteAsync(userToDelete);

            _context.StaffDetails.Remove(staffToDelete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Staff {StaffId} deleted successfully by {PerformingUser}", id, User.Identity?.Name ?? "Unknown");
            return NoContent();
        }

        private bool StaffDetailExists(int id)
        {
            return (_context.StaffDetails?.Any(e => e.StaffId == id)).GetValueOrDefault();
        }
    }
}