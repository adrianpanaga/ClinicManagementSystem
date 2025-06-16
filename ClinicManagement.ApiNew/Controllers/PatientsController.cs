using ClinicManagement.ApiNew.DTOs.Patients;
using ClinicManagement.Data.Context;
using ClinicManagement.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // For accessing user claims
using System; // Ensure System is included for DateOnly

namespace ClinicManagement.ApiNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication by default
    public class PatientsController : ControllerBase
    {
        private readonly ClinicManagementDbContext _context;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(ClinicManagementDbContext context, ILogger<PatientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of all patient profiles. Requires 'Admin' or 'HR' role.
        /// </summary>
        /// <returns>A list of PatientDetailsDto objects.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,HR,Receptionist,Nurse,Doctor")] // Roles that can view all patients
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<PatientDetailsDto>>> GetAllPatients()
        {
            var patients = await _context.Patients
                                         .Include(p => p.User)
                                         .ThenInclude(u => u!.Role) // FIX: CS8602
                                         .Select(p => new PatientDetailsDto
                                         {
                                             PatientId = p.PatientId,
                                             FirstName = p.FirstName,
                                             LastName = p.LastName,
                                             MiddleName = p.MiddleName,
                                             DateOfBirth = p.DateOfBirth,
                                             Gender = p.Gender,
                                             ContactNumber = p.ContactNumber,
                                             Email = p.Email,
                                             Address = p.Address,
                                             BloodType = p.BloodType,
                                             EmergencyContactName = p.EmergencyContactName,
                                             EmergencyContactNumber = p.EmergencyContactNumber,
                                             PhotoUrl = p.PhotoUrl,
                                             CreatedAt = p.CreatedAt,
                                             UserId = p.UserId,
                                             // If you uncomment these, apply null-coalescing or make DTO properties nullable
                                             //Username = p.User != null ? p.User.UserName : null, // Use UserName as in IdentityUser
                                             //UserEmail = p.User != null ? p.User.Email : null,   // Use Email as in IdentityUser
                                             //UserRole = (p.User != null && p.User.Role != null) ? p.User.Role.Name : null // Use Role.Name
                                         })
                                         .ToListAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Gets a specific patient by ID. Accessible by 'Admin', 'HR', or the patient themselves.
        /// </summary>
        /// <param name="id">The ID of the patient.</param>
        /// <returns>The PatientDetailsDto object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDetailsDto>> GetPatientById(int id)
        {
            var patient = await _context.Patients
                                         .Include(p => p.User)
                                             .ThenInclude(u => u!.Role) // FIX: CS8602
                                         .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            // Authorization check
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Forbid(); // Not authenticated correctly
            }

            // Check if user is Admin, HR, or if the user is the patient themselves
            bool isAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Receptionist") || User.IsInRole("Nurse") || User.IsInRole("Doctor");
            bool isSelf = int.TryParse(userIdClaim, out int currentUserId) && patient.UserId == currentUserId;

            if (!isAdminOrStaff && !isSelf)
            {
                return Forbid();
            }

            var patientDto = new PatientDetailsDto
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                MiddleName = patient.MiddleName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                ContactNumber = patient.ContactNumber,
                Email = patient.Email,
                Address = patient.Address,
                BloodType = patient.BloodType,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactNumber = patient.EmergencyContactNumber,
                PhotoUrl = patient.PhotoUrl,
                CreatedAt = patient.CreatedAt,
                UserId = patient.UserId,
                // If you uncomment these, apply null-coalescing or make DTO properties nullable
                //Username = patient.User != null ? patient.User.UserName : null,
                //UserEmail = patient.User != null ? patient.User.Email : null,
                //UserRole = (patient.User != null && patient.User.Role != null) ? patient.User.Role.Name : null
            };

            return Ok(patientDto);
        }

        /// <summary>
        /// Gets patient details by associated UserID. Accessible by 'Admin', 'HR', or the user themselves.
        /// Useful for a patient to retrieve their own record after logging in.
        /// </summary>
        /// <param name="userId">The ID of the user linked to the patient.</param>
        /// <returns>The PatientDetailsDto object.</returns>
        [HttpGet("byUser/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDetailsDto>> GetPatientByUserId(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return Forbid(); // Not authenticated correctly
            }

            // Check if user is Admin, HR, or if the requesting user is the target user
            bool isAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Receptionist") || User.IsInRole("Nurse") || User.IsInRole("Doctor");
            bool isSelf = int.TryParse(userIdClaim, out int currentUserId) && currentUserId == userId;

            if (!isAdminOrStaff && !isSelf)
            {
                return Forbid();
            }

            var patient = await _context.Patients
                                         .Include(p => p.User)
                                             .ThenInclude(u => u!.Role) // FIX: CS8602
                                         .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound($"Patient linked to User ID {userId} not found.");
            }

            var patientDto = new PatientDetailsDto
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                MiddleName = patient.MiddleName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                ContactNumber = patient.ContactNumber,
                Email = patient.Email,
                Address = patient.Address,
                BloodType = patient.BloodType,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactNumber = patient.EmergencyContactNumber,
                PhotoUrl = patient.PhotoUrl,
                CreatedAt = patient.CreatedAt,
                UserId = patient.UserId,
                // If you uncomment these, apply null-coalescing or make DTO properties nullable
                //Username = patient.User != null ? patient.User.UserName : null,
                //UserEmail = patient.User != null ? patient.User.Email : null,
                //UserRole = (patient.User != null && patient.User.Role != null) ? patient.User.Role.Name : null
            };

            return Ok(patientDto);
        }

        /// <summary>
        /// Creates a new patient profile. Requires 'Admin', 'HR', or 'Receptionist' role.
        /// </summary>
        /// <param name="createPatientDto">Patient creation details.</param>
        /// <returns>The created PatientDetailsDto object.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,Receptionist")] // Roles allowed to create new patients
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // For invalid UserId if provided
        public async Task<ActionResult<PatientDetailsDto>> CreatePatient([FromBody] CreatePatientDto createPatientDto)
        {
            // Validate if Email or ContactNumber already exist to prevent duplicates
            if (await _context.Patients.AnyAsync(p => p.Email == createPatientDto.Email))
            {
                return BadRequest("Patient with this email already exists.");
            }
            if (await _context.Patients.AnyAsync(p => p.ContactNumber == createPatientDto.ContactNumber))
            {
                return BadRequest("Patient with this contact number already exists.");
            }

            // If a UserId is provided, ensure the User exists and is not already linked to a patient
            User? linkedUser = null;
            if (createPatientDto.UserId.HasValue)
            {
                linkedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == createPatientDto.UserId.Value);
                if (linkedUser == null)
                {
                    return NotFound($"User with ID {createPatientDto.UserId.Value} not found.");
                }
                if (await _context.Patients.AnyAsync(p => p.UserId == createPatientDto.UserId.Value))
                {
                    return BadRequest($"User with ID {createPatientDto.UserId.Value} is already linked to another patient.");
                }
            }

            var newPatient = new Patient
            {
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                MiddleName = createPatientDto.MiddleName,
                DateOfBirth = createPatientDto.DateOfBirth,
                Gender = createPatientDto.Gender,
                ContactNumber = createPatientDto.ContactNumber,
                Email = createPatientDto.Email,
                Address = createPatientDto.Address,
                BloodType = createPatientDto.BloodType,
                EmergencyContactName = createPatientDto.EmergencyContactName,
                EmergencyContactNumber = createPatientDto.EmergencyContactNumber,
                PhotoUrl = createPatientDto.PhotoUrl,
                CreatedAt = DateTime.UtcNow,
                UserId = createPatientDto.UserId
            };

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            // Populate navigation property for DTO
            if (linkedUser != null)
            {
                newPatient.User = linkedUser;
                // If Role needs to be included for linkedUser, ensure it's loaded
                if (linkedUser.Role == null)
                {
                    await _context.Entry(linkedUser).Reference(u => u.Role).LoadAsync();
                }
            }

            var patientDto = new PatientDetailsDto
            {
                PatientId = newPatient.PatientId,
                FirstName = newPatient.FirstName,
                LastName = newPatient.LastName,
                MiddleName = newPatient.MiddleName,
                DateOfBirth = newPatient.DateOfBirth,
                Gender = newPatient.Gender,
                ContactNumber = newPatient.ContactNumber,
                Email = newPatient.Email,
                Address = newPatient.Address,
                BloodType = newPatient.BloodType,
                EmergencyContactName = newPatient.EmergencyContactName,
                EmergencyContactNumber = newPatient.EmergencyContactNumber,
                PhotoUrl = newPatient.PhotoUrl,
                CreatedAt = newPatient.CreatedAt,
                UserId = newPatient.UserId,
                // If you uncomment these, apply null-coalescing or make DTO properties nullable
                //Username = newPatient.User != null ? newPatient.User.UserName : null,
                //UserEmail = newPatient.User != null ? newPatient.User.Email : null,
                //UserRole = (newPatient.User != null && newPatient.User.Role != null) ? newPatient.User.Role.Name : null
            };

            _logger.LogInformation("Patient created: {PatientFullName} (ID: {PatientId}), linked to User ID: {UserId}",
                                   $"{newPatient.FirstName} {newPatient.LastName}", newPatient.PatientId, newPatient.UserId);

            return CreatedAtAction(nameof(GetPatientById), new { id = patientDto.PatientId }, patientDto);
        }

        /// <summary>
        /// Updates an existing patient's details. Accessible by 'Admin', 'HR', or the patient themselves.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="updatePatientDto">Updated patient details.</param>
        /// <returns>No content on success.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            var patientToUpdate = await _context.Patients
                                                .Include(p => p.User)
                                                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patientToUpdate == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            // Authorization check
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Forbid();
            }

            bool isAdminOrStaff = User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Receptionist");
            bool isSelf = int.TryParse(userIdClaim, out int currentUserId) && patientToUpdate.UserId == currentUserId;

            if (!isAdminOrStaff && !isSelf)
            {
                return Forbid("You are not authorized to update this patient's details.");
            }

            // Validate email unique if updated
            if (updatePatientDto.Email != null && await _context.Patients.AnyAsync(p => p.Email == updatePatientDto.Email && p.PatientId != id))
            {
                return BadRequest("Email already in use by another patient.");
            }
            // Validate contact number unique if updated
            if (updatePatientDto.ContactNumber != null && await _context.Patients.AnyAsync(p => p.ContactNumber == updatePatientDto.ContactNumber && p.PatientId != id))
            {
                return BadRequest("Contact number already in use by another patient.");
            }

            // If UserId is being updated, ensure new UserId exists and is not linked to another patient
            if (updatePatientDto.UserId.HasValue && patientToUpdate.UserId != updatePatientDto.UserId.Value)
            {
                var newLinkedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == updatePatientDto.UserId.Value);
                if (newLinkedUser == null)
                {
                    return NotFound($"User with ID {updatePatientDto.UserId.Value} not found for linking.");
                }
                if (await _context.Patients.AnyAsync(p => p.UserId == updatePatientDto.UserId.Value && p.PatientId != id))
                {
                    return BadRequest($"User with ID {updatePatientDto.UserId.Value} is already linked to another patient.");
                }
                patientToUpdate.UserId = updatePatientDto.UserId.Value;
            }
            // If setting UserId to 0 explicitly, meaning no linked user, set to null for nullable FK
            // This condition is simplified if UserId is directly assigned below based on HasValue
            else if (updatePatientDto.UserId.HasValue && updatePatientDto.UserId.Value == 0)
            {
                patientToUpdate.UserId = null; // Explicitly set to null if client sends 0 for unlinking
            }
            // Add a case for if updatePatientDto.UserId is explicitly null from the client
            else if (!updatePatientDto.UserId.HasValue)
            {
                patientToUpdate.UserId = null; // Explicitly unlink if client sends null
            }


            // Apply updates
            if (updatePatientDto.FirstName != null) patientToUpdate.FirstName = updatePatientDto.FirstName;
            if (updatePatientDto.LastName != null) patientToUpdate.LastName = updatePatientDto.LastName;
            if (updatePatientDto.MiddleName != null) patientToUpdate.MiddleName = updatePatientDto.MiddleName;
            if (updatePatientDto.DateOfBirth.HasValue) patientToUpdate.DateOfBirth = updatePatientDto.DateOfBirth.Value;
            if (updatePatientDto.Gender != null) patientToUpdate.Gender = updatePatientDto.Gender;
            if (updatePatientDto.ContactNumber != null) patientToUpdate.ContactNumber = updatePatientDto.ContactNumber;
            if (updatePatientDto.Email != null) patientToUpdate.Email = updatePatientDto.Email;
            if (updatePatientDto.Address != null) patientToUpdate.Address = updatePatientDto.Address;
            if (updatePatientDto.BloodType != null) patientToUpdate.BloodType = updatePatientDto.BloodType;
            if (updatePatientDto.EmergencyContactName != null) patientToUpdate.EmergencyContactName = updatePatientDto.EmergencyContactName;
            if (updatePatientDto.EmergencyContactNumber != null) patientToUpdate.EmergencyContactNumber = updatePatientDto.EmergencyContactNumber;
            if (updatePatientDto.PhotoUrl != null) patientToUpdate.PhotoUrl = updatePatientDto.PhotoUrl;


            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Patient {PatientId} updated by {PerformingUser}", id, User.Identity?.Name ?? "Unknown");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Patients.AnyAsync(e => e.PatientId == id))
                {
                    return NotFound($"Patient with ID {id} not found after update attempt.");
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {PatientId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the patient.");
            }
        }

        /// <summary>
        /// Deletes a patient profile. Requires 'Admin' or 'HR' role.
        /// This is a hard delete and should be used with caution. Consider soft-delete (e.g., IsActive status) instead.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,HR")] // Only Admin or HR can delete patient records
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patientToDelete = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id);

            if (patientToDelete == null)
            {
                return NotFound($"Patient with ID {id} not found.");
            }

            // Before deleting, consider if you need to handle cascade deletes or set foreign keys to null
            // For example, what happens to Appointments or MedicalRecords linked to this patient?
            // Your EF Core configuration defines cascade behavior, so review that carefully.

            _context.Patients.Remove(patientToDelete);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Patient {PatientId} deleted by {PerformingUser}", id, User.Identity?.Name ?? "Unknown");
            return NoContent();
        }
    }
}