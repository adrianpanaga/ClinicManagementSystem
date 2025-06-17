// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.ApiNew\Controllers\MedicalRecordsController.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using ClinicManagement.Data.Context;
using ClinicManagement.Data.Models;
using ClinicManagement.ApiNew.DTOs.MedicalRecords; // Import MedicalRecords DTOs
using ClinicManagement.ApiNew.DTOs.Appointments; // For AppointmentDto
using ClinicManagement.ApiNew.DTOs.Patients;    // For PatientDetailsDto
using ClinicManagement.ApiNew.DTOs.StaffDetails; // For StaffDetailDto
// If you have a Services DTO, add its using here, e.g.:
// using ClinicManagement.ApiNew.DTOs.Services;

namespace ClinicManagement.ApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions in this controller require an authenticated user by default
    public class MedicalRecordsController : ControllerBase
    {
        private readonly ClinicManagementDbContext _context;

        public MedicalRecordsController(ClinicManagementDbContext context)
        {
            _context = context;
        }

        // Helper method to map MedicalRecord model to MedicalRecordsDto
        private MedicalRecordsDto MapToMedicalRecordsDto(MedicalRecord medicalRecord)
        {
            return new MedicalRecordsDto
            {
                RecordId = medicalRecord.RecordId,
                PatientId = medicalRecord.PatientId,
                AppointmentId = medicalRecord.AppointmentId,
                StaffId = medicalRecord.StaffId,
                ServiceId = medicalRecord.ServiceId,
                // FIX: Use null-coalescing to assign an empty string if null, resolving CS8601.
                Diagnosis = medicalRecord.Diagnosis ?? "",
                Treatment = medicalRecord.Treatment ?? "",
                Prescription = medicalRecord.Prescription ?? "",
                CreatedAt = medicalRecord.CreatedAt,
                UpdatedAt = medicalRecord.UpdatedAt,
                Patient = medicalRecord.Patient != null ? new PatientDetailsDto // Assuming PatientDetailsDto
                {
                    PatientId = medicalRecord.Patient.PatientId,
                    FirstName = medicalRecord.Patient.FirstName,
                    MiddleName = medicalRecord.Patient.MiddleName,
                    LastName = medicalRecord.Patient.LastName,
                    Gender = medicalRecord.Patient.Gender,
                    DateOfBirth = medicalRecord.Patient.DateOfBirth != null ? DateOnly.FromDateTime(medicalRecord.Patient.DateOfBirth.Value) : (DateOnly?)null,
                    Address = medicalRecord.Patient.Address,
                    ContactNumber = medicalRecord.Patient.ContactNumber,
                    Email = medicalRecord.Patient.Email,
                    BloodType = medicalRecord.Patient.BloodType,
                    EmergencyContactName = medicalRecord.Patient.EmergencyContactName,
                    EmergencyContactNumber = medicalRecord.Patient.EmergencyContactNumber,
                    PhotoUrl = medicalRecord.Patient.PhotoUrl,
                    CreatedAt = medicalRecord.Patient.CreatedAt,
                    UpdatedAt = medicalRecord.Patient.UpdatedAt,
                    UserId = medicalRecord.Patient.UserId,
                    IsDeleted = medicalRecord.Patient.IsDeleted
                } : null,
                Staff = medicalRecord.Staff != null ? new StaffDetailDto
                {
                    StaffId = medicalRecord.Staff.StaffId,
                    FirstName = medicalRecord.Staff.FirstName,
                    LastName = medicalRecord.Staff.LastName,
                    JobTitle = medicalRecord.Staff.JobTitle,
                    Specialization = medicalRecord.Staff.Specialization
                    // Map other properties as needed from your StaffDetail model
                } : null,
                Appointment = medicalRecord.Appointment != null ? new AppointmentDto
                {
                    AppointmentId = medicalRecord.Appointment.AppointmentId,
                    AppointmentDateTime = medicalRecord.Appointment.AppointmentDateTime,
                    Status = medicalRecord.Appointment.Status ?? "",
                    PatientId = medicalRecord.Appointment.PatientId ?? 0,
                    DoctorId = medicalRecord.Appointment.DoctorId
                    // Map other properties as needed from your Appointment model
                } : null,
                // Uncomment and map if you have a ServiceDto and want to include it.
                // You'll need to add 'using ClinicManagement.ApiNew.DTOs.Services;' above.
                // Service = medicalRecord.Service != null ? new ServiceDto
                // {
                //     ServiceId = medicalRecord.Service.ServiceId,
                //     ServiceName = medicalRecord.Service.ServiceName,
                //     Description = medicalRecord.Service.Description,
                //     Price = medicalRecord.Service.Price
                // } : null
            };
        }


        // GET: api/MedicalRecords
        /// <summary>
        /// Retrieves all medical records. For Admins/HR, can optionally include deleted records.
        /// Requires Admin or HR role to see all records, otherwise limited by user role.
        /// </summary>
        /// <param name="includeDeleted">Optional. If true and user is Admin/HR, includes soft-deleted records.</param>
        /// <returns>A list of MedicalRecordsDto.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,HR")] // Only Admin and HR can view all medical records (including option for deleted)
        public async Task<ActionResult<IEnumerable<MedicalRecordsDto>>> GetMedicalRecords([FromQuery] bool includeDeleted = false)
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }

            IQueryable<MedicalRecord> query = _context.MedicalRecords;

            // If includeDeleted is true AND the user is Admin or HR, bypass the global query filter
            if (includeDeleted && (User.IsInRole("Admin") || User.IsInRole("HR")))
            {
                query = query.IgnoreQueryFilters();
            }
            // For other roles or if includeDeleted is false, the global filter (IsDeleted = false) applies by default.

            var medicalRecords = await query
                                 .Include(mr => mr.Patient)
                                 .Include(mr => mr.Staff)
                                 .Include(mr => mr.Appointment)
                                 .Include(mr => mr.Service)
                                 .ToListAsync();

            return medicalRecords.Select(mr => MapToMedicalRecordsDto(mr)).ToList();
        }

        // GET: api/MedicalRecords/5
        /// <summary>
        /// Retrieves a specific medical record by its ID.
        /// Requires Admin, Doctor, Nurse, or the Patient linked to the record.
        /// Admins/HR can retrieve deleted records by ID.
        /// </summary>
        /// <param name="id">The ID of the medical record.</param>
        /// <returns>The MedicalRecordsDto if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Doctor,Nurse,Patient")]
        public async Task<ActionResult<MedicalRecordsDto>> GetMedicalRecord(int id)
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }

            IQueryable<MedicalRecord> query = _context.MedicalRecords;

            // Admins/HR can retrieve deleted records by ID
            if (User.IsInRole("Admin") || User.IsInRole("HR"))
            {
                query = query.IgnoreQueryFilters();
            }

            var medicalRecord = await query
                                              .Include(mr => mr.Patient)
                                              .Include(mr => mr.Staff)
                                              .Include(mr => mr.Appointment)
                                              .Include(mr => mr.Service)
                                              .FirstOrDefaultAsync(mr => mr.RecordId == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            // Custom authorization logic for Patients:
            // If the user is in the "Patient" role, ensure they are only requesting their own record.
            if (User.IsInRole("Patient"))
            {
                // Get the UserID from the authenticated user's claims
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized("Could not identify current user.");
                }

                // Check if the current user (patient) is linked to this medical record's patient
                // This assumes Patient model has a UserId property linking to IdentityUser.Id
                var patientLinkedToRecord = await _context.Patients
                                                            .Where(p => p.PatientId == medicalRecord.PatientId)
                                                            .Select(p => p.UserId)
                                                            .FirstOrDefaultAsync();

                if (patientLinkedToRecord == null || patientLinkedToRecord != currentUserId)
                {
                    return Forbid("You do not have access to this medical record.");
                }
            }

            return MapToMedicalRecordsDto(medicalRecord);
        }

        // GET: api/MedicalRecords/patient/{patientId}
        /// <summary>
        /// Retrieves all medical records for a specific patient.
        /// Requires Admin, Doctor, Nurse, or the specific Patient.
        /// Admins/HR can optionally include deleted records for a patient.
        /// </summary>
        /// <param name="patientId">The ID of the patient.</param>
        /// <param name="includeDeleted">Optional. If true and user is Admin/HR, includes soft-deleted records.</param>
        /// <returns>A list of MedicalRecordsDto for the specified patient.</returns>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Admin,Doctor,Nurse,Patient")]
        public async Task<ActionResult<IEnumerable<MedicalRecordsDto>>> GetMedicalRecordsByPatient(int patientId, [FromQuery] bool includeDeleted = false)
        {
            // Custom authorization logic for Patients:
            if (User.IsInRole("Patient"))
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized("Could not identify current user.");
                }

                // Find the patient ID associated with the current user ID
                var patientIdForCurrentUser = await _context.Patients
                                                              .Where(p => p.UserId == currentUserId)
                                                              .Select(p => p.PatientId)
                                                              .FirstOrDefaultAsync();

                if (patientIdForCurrentUser == 0 || patientIdForCurrentUser != patientId)
                {
                    return Forbid("You can only view your own medical records.");
                }
            }

            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }

            IQueryable<MedicalRecord> query = _context.MedicalRecords;

            // Admins/HR can retrieve deleted records for a specific patient
            if (includeDeleted && (User.IsInRole("Admin") || User.IsInRole("HR")))
            {
                query = query.IgnoreQueryFilters();
            }

            var medicalRecords = await query
                                 .Where(mr => mr.PatientId == patientId)
                                 .Include(mr => mr.Patient)
                                 .Include(mr => mr.Staff)
                                 .Include(mr => mr.Appointment)
                                 .Include(mr => mr.Service)
                                 .ToListAsync();

            if (!medicalRecords.Any())
            {
                return NotFound($"No medical records found for patient ID {patientId}.");
            }

            return medicalRecords.Select(mr => MapToMedicalRecordsDto(mr)).ToList();
        }

        // GET: api/MedicalRecords/doctor/{doctorId}
        /// <summary>
        /// Retrieves all medical records created or managed by a specific doctor (staff member).
        /// Requires Admin, HR, Doctor (their own records), or Nurse (their own created records).
        /// Admins/HR can optionally include deleted records for a doctor.
        /// </summary>
        /// <param name="doctorId">The ID of the staff member (doctor).</param>
        /// <param name="includeDeleted">Optional. If true and user is Admin/HR, includes soft-deleted records.</param>
        /// <returns>A list of MedicalRecordsDto created by the specified doctor.</returns>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,HR,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<MedicalRecordsDto>>> GetMedicalRecordsByDoctor(int doctorId, [FromQuery] bool includeDeleted = false)
        {
            // Custom authorization logic for Doctors/Nurses:
            if (User.IsInRole("Doctor") || User.IsInRole("Nurse"))
            {
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (currentUserIdClaim == null || !int.TryParse(currentUserIdClaim.Value, out int currentUserId))
                {
                    return Unauthorized("Could not identify current user.");
                }

                var staffIdForCurrentUser = await _context.StaffDetails
                                                            .Where(sd => sd.UserId == currentUserId)
                                                            .Select(sd => sd.StaffId)
                                                            .FirstOrDefaultAsync();

                if (staffIdForCurrentUser == 0 || staffIdForCurrentUser != doctorId)
                {
                    return Forbid("You can only view medical records associated with your own Staff ID.");
                }
            }

            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }

            IQueryable<MedicalRecord> query = _context.MedicalRecords;

            // Admins/HR can retrieve deleted records for a specific doctor
            if (includeDeleted && (User.IsInRole("Admin") || User.IsInRole("HR")))
            {
                query = query.IgnoreQueryFilters();
            }

            var medicalRecords = await query
                                 .Where(mr => mr.StaffId == doctorId)
                                 .Include(mr => mr.Patient)
                                 .Include(mr => mr.Staff)
                                 .Include(mr => mr.Appointment)
                                 .Include(mr => mr.Service)
                                 .ToListAsync();

            if (!medicalRecords.Any())
            {
                return NotFound($"No medical records found for doctor (staff) ID {doctorId}.");
            }

            return medicalRecords.Select(mr => MapToMedicalRecordsDto(mr)).ToList();
        }


        // PUT: api/MedicalRecords/5
        /// <summary>
        /// Updates an existing medical record.
        /// Requires Admin, Doctor, or Nurse role.
        /// </summary>
        /// <param name="id">The ID of the medical record to update.</param>
        /// <param name="updateMedicalRecordsDto">The DTO object with updated data.</param>
        /// <returns>NoContent if successful, BadRequest if ID mismatch, NotFound if record not found, or throws exception for concurrency.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<IActionResult> PutMedicalRecord(int id, UpdateMedicalRecordsDto updateMedicalRecordsDto)
        {
            if (id != updateMedicalRecordsDto.RecordId)
            {
                return BadRequest("Mismatched Record ID in route and body.");
            }

            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
                return NotFound();


            medicalRecord.PatientId = updateMedicalRecordsDto.PatientId;
            medicalRecord.AppointmentId = updateMedicalRecordsDto.AppointmentId ?? 0;
            medicalRecord.StaffId = updateMedicalRecordsDto.StaffId;
            medicalRecord.ServiceId = updateMedicalRecordsDto.ServiceId ?? 0;
            medicalRecord.Diagnosis = updateMedicalRecordsDto.Diagnosis;
            medicalRecord.Treatment = updateMedicalRecordsDto.Treatment;
            medicalRecord.Prescription = updateMedicalRecordsDto.Prescription;
            medicalRecord.UpdatedAt = DateTime.UtcNow;

            _context.Entry(medicalRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalRecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MedicalRecords
        /// <summary>
        /// Creates a new medical record.
        /// Requires Admin, Doctor, or Nurse role.
        /// </summary>
        /// <param name="createMedicalRecordsDto">The DTO object to create.</param>
        /// <returns>The created MedicalRecordsDto with its ID, or a problem if the DbSet is null.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<ActionResult<MedicalRecordsDto>> PostMedicalRecord(CreateMedicalRecordsDto createMedicalRecordsDto)
        {
            if (_context.MedicalRecords == null)
            {
                return Problem("Entity set 'ClinicManagementDbContext.MedicalRecords' is null.");
            }

            var medicalRecord = new MedicalRecord
            {
                PatientId = createMedicalRecordsDto.PatientId,
                AppointmentId = createMedicalRecordsDto.AppointmentId ?? 0,
                StaffId = createMedicalRecordsDto.StaffId,
                ServiceId = createMedicalRecordsDto.ServiceId ?? 0,
                Diagnosis = createMedicalRecordsDto.Diagnosis,
                Treatment = createMedicalRecordsDto.Treatment,
                Prescription = createMedicalRecordsDto.Prescription,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            await _context.Entry(medicalRecord)
                          .Reference(mr => mr.Patient).LoadAsync();
            await _context.Entry(medicalRecord)
                          .Reference(mr => mr.Staff).LoadAsync();
            await _context.Entry(medicalRecord)
                          .Reference(mr => mr.Appointment).LoadAsync();
            await _context.Entry(medicalRecord)
                          .Reference(mr => mr.Service).LoadAsync();


            return CreatedAtAction("GetMedicalRecord", new { id = medicalRecord.RecordId }, MapToMedicalRecordsDto(medicalRecord));
        }

        // DELETE: api/MedicalRecords/5 (Soft Delete)
        /// <summary>
        /// Soft deletes a medical record by marking it as IsDeleted = true.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the medical record to soft delete.</param>
        /// <returns>NoContent if successful, or NotFound if the record does not exist or is already deleted.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteMedicalRecord(int id)
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }
            var medicalRecord = await _context.MedicalRecords.IgnoreQueryFilters().FirstOrDefaultAsync(mr => mr.RecordId == id);
            if (medicalRecord == null)
            {
                return NotFound("Medical record not found.");
            }

            if (medicalRecord.IsDeleted)
            {
                return BadRequest("Medical record is already soft-deleted.");
            }

            medicalRecord.IsDeleted = true;
            medicalRecord.UpdatedAt = DateTime.UtcNow;

            _context.Entry(medicalRecord).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/MedicalRecords/restore/5
        /// <summary>
        /// Restores a soft-deleted medical record by setting IsDeleted = false.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the medical record to restore.</param>
        /// <returns>NoContent if successful, or NotFound if the record does not exist or is not deleted.</returns>
        [HttpPost("restore/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RestoreMedicalRecord(int id)
        {
            if (_context.MedicalRecords == null)
            {
                return NotFound();
            }
            var medicalRecord = await _context.MedicalRecords.IgnoreQueryFilters().FirstOrDefaultAsync(mr => mr.RecordId == id);
            if (medicalRecord == null)
            {
                return NotFound("Medical record not found.");
            }

            if (!medicalRecord.IsDeleted)
            {
                return BadRequest("Medical record is not soft-deleted.");
            }

            medicalRecord.IsDeleted = false;
            medicalRecord.UpdatedAt = DateTime.UtcNow;

            _context.Entry(medicalRecord).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool MedicalRecordExists(int id)
        {
            return (_context.MedicalRecords?.Any(e => e.RecordId == id)).GetValueOrDefault();
        }
    }
}
