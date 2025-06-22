// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.ApiNew\Controllers\AppointmentsController.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // For authorization attributes
using System.Security.Claims; // For accessing user claims (e.g., UserId)
using ClinicManagement.Data;
using ClinicManagement.Data.Context; // Your DbContext
using ClinicManagement.Data.Models;   // Your EF Core models
using ClinicManagement.ApiNew.DTOs.Appointments; // Your Appointment DTOs
using ClinicManagement.ApiNew.DTOs.Patients;     // For PatientDetailsDto
using ClinicManagement.ApiNew.DTOs.StaffDetails; // For StaffDetailDto (Doctor)
using ClinicManagement.ApiNew.DTOs.Services;    // For ServiceDto

namespace ClinicManagement.ApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions in this controller require an authenticated user by default
    public class AppointmentsController : ControllerBase
    {
        private readonly ClinicManagementDbContext _context;

        public AppointmentsController(ClinicManagementDbContext context)
        {
            _context = context;
        }

        // Helper method to map Appointment model to AppointmentDto
        private static AppointmentDto MapToAppointmentDto(Appointment appointment)
        {
            if (appointment == null) return null; // CS8603: Expected null return.

            return new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = (int)appointment.DoctorId,
                ServiceId = appointment.ServiceId,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                // Manually map related entities to their respective DTOs
                Patient = appointment.Patient != null ? new PatientDetailsDto
                {
                    PatientId = appointment.Patient.PatientId,
                    FirstName = appointment.Patient.FirstName,
                    MiddleName = appointment.Patient.MiddleName,
                    LastName = appointment.Patient.LastName,
                    Gender = appointment.Patient.Gender,
                    DateOfBirth = appointment.Patient.DateOfBirth.HasValue ? DateOnly.FromDateTime(appointment.Patient.DateOfBirth.Value) : (DateOnly?)null,
                    Address = appointment.Patient.Address,
                    ContactNumber = appointment.Patient.ContactNumber,
                    Email = appointment.Patient.Email,
                    BloodType = appointment.Patient.BloodType,
                    EmergencyContactName = appointment.Patient.EmergencyContactName,
                    EmergencyContactNumber = appointment.Patient.EmergencyContactNumber,
                    PhotoUrl = appointment.Patient.PhotoUrl,
                    CreatedAt = appointment.Patient.CreatedAt,
                    UpdatedAt = appointment.Patient.UpdatedAt,
                    UserId = appointment.Patient.UserId,
                    IsDeleted = appointment.Patient.IsDeleted
                } : null,
                Doctor = appointment.Doctor != null ? new StaffDetailDto
                {
                    StaffId = appointment.Doctor.StaffId,
                    FirstName = appointment.Doctor.FirstName,
                    MiddleName = appointment.Doctor.MiddleName,
                    LastName = appointment.Doctor.LastName,
                    JobTitle = appointment.Doctor.JobTitle,
                    Specialization = appointment.Doctor.Specialization,
                    ContactNumber = appointment.Doctor.ContactNumber,
                    Email = appointment.Doctor.Email,
                    CreatedAt = appointment.Doctor.CreatedAt,
                    UpdatedAt = appointment.Doctor.UpdatedAt,
                    UserId = appointment.Doctor.UserId,
                    IsDeleted = appointment.Doctor.IsDeleted
                } : null,
                Service = appointment.Service != null ? new ServiceDto
                {
                    ServiceId = appointment.Service.ServiceId,
                    ServiceName = appointment.Service.ServiceName,
                    Description = appointment.Service.Description,
                    Price = appointment.Service.Price,
                    CreatedAt = appointment.Service.CreatedAt,
                    UpdatedAt = appointment.Service.UpdatedAt
                } : null
            };
        }

        // GET: api/Appointments
        /// <summary>
        /// Retrieves all appointments. Accessible by Admins, HR, Receptionists. Doctors/Nurses/Patients see filtered results.
        /// </summary>
        /// <returns>A list of AppointmentDto.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,HR,Receptionist,Doctor,Nurse,Patient")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            if (_context.Appointments == null)
            {
                return NotFound();
            }

            IQueryable<Appointment> query = _context.Appointments
                                                .Include(a => a.Patient)
                                                .Include(a => a.Doctor)
                                                .Include(a => a.Service);

            var appointments = await query.ToListAsync();

            // Apply specific authorization filters based on role if not Admin/HR/Receptionist
            if (!(User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Receptionist")))
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized("Could not identify current user.");
                }

                if (User.IsInRole("Patient"))
                {
                    var patientIdForCurrentUser = await _context.Patients
                                                                  .Where(p => p.UserId == currentUserId)
                                                                  .Select(p => p.PatientId)
                                                                  .FirstOrDefaultAsync();
                    appointments = appointments.Where(a => a.PatientId == patientIdForCurrentUser).ToList();
                }
                else if (User.IsInRole("Doctor") || User.IsInRole("Nurse"))
                {
                    var staffIdForCurrentUser = await _context.StaffDetails
                                                                .Where(sd => sd.UserId == currentUserId)
                                                                .Select(sd => sd.StaffId)
                                                                .FirstOrDefaultAsync();
                    appointments = appointments.Where(a => a.DoctorId == staffIdForCurrentUser).ToList();
                }
                else
                {
                    return Forbid("You do not have sufficient permissions to view appointments.");
                }
            }

            return appointments.Select(a => MapToAppointmentDto(a)).ToList();
        }

        // GET: api/Appointments/5
        /// <summary>
        /// Retrieves a specific appointment by ID.
        /// Requires Admin, HR, Receptionist, or if patient/doctor/nurse, then it must be their own.
        /// </summary>
        /// <param name="id">The ID of the appointment.</param>
        /// <returns>The AppointmentDto if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,HR,Receptionist,Doctor,Nurse,Patient")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            if (_context.Appointments == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                                            .Include(a => a.Patient)
                                            .Include(a => a.Doctor)
                                            .Include(a => a.Service)
                                            .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Custom authorization logic for Patients, Doctors, Nurses:
            if (!(User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Receptionist")))
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized("Could not identify current user.");
                }

                if (User.IsInRole("Patient"))
                {
                    var patientLinkedToAppointment = await _context.Patients
                                                            .Where(p => p.PatientId == appointment.PatientId)
                                                            .Select(p => p.UserId)
                                                            .FirstOrDefaultAsync();
                    if (patientLinkedToAppointment == null || patientLinkedToAppointment != currentUserId)
                    {
                        return Forbid("You do not have access to this appointment record.");
                    }
                }
                else if (User.IsInRole("Doctor") || User.IsInRole("Nurse"))
                {
                    var staffIdForCurrentUser = await _context.StaffDetails
                                                                .Where(sd => sd.UserId == currentUserId)
                                                                .Select(sd => sd.StaffId)
                                                                .FirstOrDefaultAsync();
                    if (staffIdForCurrentUser == 0 || appointment.DoctorId != staffIdForCurrentUser)
                    {
                        return Forbid("You do not have access to this appointment record.");
                    }
                }
                else
                {
                    return Forbid("You do not have sufficient permissions to view this appointment.");
                }
            }

            return MapToAppointmentDto(appointment);
        }

        // PUT: api/Appointments/5
        /// <summary>
        /// Updates an existing appointment.
        /// Requires Admin, HR, or Receptionist role.
        /// </summary>
        /// <param name="id">The ID of the appointment to update.</param>
        /// <param name="updateAppointmentDto">The DTO object with updated data.</param>
        /// <returns>NoContent if successful, BadRequest if ID mismatch, NotFound if appointment not found, or throws exception for concurrency.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR,Receptionist")] // Roles allowed to update appointments
        public async Task<IActionResult> PutAppointment(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            if (id != updateAppointmentDto.AppointmentId)
            {
                return BadRequest("Mismatched Appointment ID in route and body.");
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Validate foreign keys exist and are not soft-deleted if updated
            if (updateAppointmentDto.PatientId.HasValue && updateAppointmentDto.PatientId != appointment.PatientId)
            {
                if (!await _context.Patients.AnyAsync(p => p.PatientId == updateAppointmentDto.PatientId.Value && !p.IsDeleted))
                {
                    return BadRequest($"Patient with ID {updateAppointmentDto.PatientId.Value} does not exist or is deleted.");
                }
            }
            if (updateAppointmentDto.DoctorId.HasValue && updateAppointmentDto.DoctorId != appointment.DoctorId)
            {
                if (!await _context.StaffDetails.AnyAsync(s => s.StaffId == updateAppointmentDto.DoctorId.Value && !s.IsDeleted))
                {
                    return BadRequest($"Doctor (Staff) with ID {updateAppointmentDto.DoctorId.Value} does not exist or is deleted.");
                }
            }
            if (updateAppointmentDto.ServiceId.HasValue && updateAppointmentDto.ServiceId != appointment.ServiceId)
            {
                if (!await _context.Services.AnyAsync(s => s.ServiceId == updateAppointmentDto.ServiceId.Value))
                {
                    return BadRequest($"Service with ID {updateAppointmentDto.ServiceId.Value} does not exist.");
                }
            }

            // Manually map properties from DTO to the existing EF Core entity
            appointment.PatientId = updateAppointmentDto.PatientId;
            appointment.DoctorId = updateAppointmentDto.DoctorId ?? appointment.DoctorId; // DoctorId is non-nullable in model, needs coalescing
            appointment.ServiceId = updateAppointmentDto.ServiceId ?? appointment.ServiceId; // ServiceId is non-nullable in model, needs coalescing
            appointment.AppointmentDateTime = updateAppointmentDto.AppointmentDateTime ?? appointment.AppointmentDateTime; // DateTime is non-nullable, needs coalescing
            appointment.Status = updateAppointmentDto.Status ?? appointment.Status;
            appointment.Notes = updateAppointmentDto.Notes ?? appointment.Notes;
            appointment.UpdatedAt = DateTime.UtcNow; // Set update timestamp

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw if it's a genuine concurrency issue
                }
            }

            return NoContent();
        }

        // POST: api/Appointments
        /// <summary>
        /// Creates a new appointment.
        /// Requires Admin, HR, or Receptionist role.
        /// </summary>
        /// <param name="createAppointmentDto">The DTO object to create.</param>
        /// <returns>The created AppointmentDto with its ID, or a problem if the DbSet is null.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,Receptionist")] // Roles allowed to create appointments
        public async Task<ActionResult<AppointmentDto>> PostAppointment(CreateAppointmentDto createAppointmentDto)
        {
            if (_context.Appointments == null)
            {
                return Problem("Entity set 'ClinicManagementDbContext.Appointments' is null.");
            }

            // Validate foreign keys exist and are not soft-deleted
            if (createAppointmentDto.PatientId.HasValue && !await _context.Patients.AnyAsync(p => p.PatientId == createAppointmentDto.PatientId.Value && !p.IsDeleted))
            {
                return BadRequest($"Patient with ID {createAppointmentDto.PatientId.Value} does not exist or is deleted.");
            }
            if (!await _context.StaffDetails.AnyAsync(s => s.StaffId == createAppointmentDto.DoctorId && !s.IsDeleted))
            {
                return BadRequest($"Doctor (Staff) with ID {createAppointmentDto.DoctorId} does not exist or is deleted.");
            }
            if (!await _context.Services.AnyAsync(s => s.ServiceId == createAppointmentDto.ServiceId))
            {
                return BadRequest($"Service with ID {createAppointmentDto.ServiceId} does not exist.");
            }


            // Manually map DTO to EF Core model
            var appointment = new Appointment
            {
                PatientId = createAppointmentDto.PatientId,
                DoctorId = createAppointmentDto.DoctorId,
                ServiceId = createAppointmentDto.ServiceId,
                AppointmentDateTime = createAppointmentDto.AppointmentDateTime,
                Status = createAppointmentDto.Status,
                Notes = createAppointmentDto.Notes,
                CreatedAt = DateTime.UtcNow, // Set creation timestamp
                UpdatedAt = DateTime.UtcNow  // Set initial update timestamp
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Load related entities before mapping to DTO for response
            await _context.Entry(appointment).Reference(a => a.Patient).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Doctor).LoadAsync();
            await _context.Entry(appointment).Reference(a => a.Service).LoadAsync();


            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, MapToAppointmentDto(appointment));
        }

        // DELETE: api/Appointments/5 (Hard Delete)
        /// <summary>
        /// Deletes an appointment by its ID. Note: This is a hard delete.
        /// Requires Admin or HR role.
        /// </summary>
        /// <param name="id">The ID of the appointment to delete.</param>
        /// <returns>NoContent if successful, or NotFound if the appointment does not exist.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,HR")] // Only Admin or HR can delete appointments
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (_context.Appointments == null)
            {
                return NotFound();
            }
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent(); // Indicate successful deletion
        }

        private bool AppointmentExists(int id)
        {
            return (_context.Appointments?.Any(e => e.AppointmentId == id)).GetValueOrDefault();
        }
    }
}
