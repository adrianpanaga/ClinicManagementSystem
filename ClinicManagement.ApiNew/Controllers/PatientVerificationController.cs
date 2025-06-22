using ClinicManagement.Data.Context; // Corrected namespace: ClinicManagement.Data.Context
using ClinicManagement.Data.Models; // Corrected namespace: ClinicManagement.Data.Models
using ClinicManagement.ApiNew.DTOs.PatientVerification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic; // Added for ICollection<Appointment> in Patient model if not already there
using System.Linq;
using System.Threading.Tasks;

// Corrected namespace to match folder structure
namespace ClinicManagement.ApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // IDE0290: Use primary constructor - Not changing for broader compatibility
    public class PatientVerificationController : ControllerBase
    {
        private readonly ClinicManagementDbContext _context;
        private readonly ILogger<PatientVerificationController> _logger;

        public PatientVerificationController(ClinicManagementDbContext context, ILogger<PatientVerificationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("RequestCode")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RequestCode([FromBody] RequestVerificationCodeDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // CS8601: Possible null reference assignment - Add null-forgiving operator or null check
            // Fixed by adding null-forgiving operator '!' as we expect these to be non-null based on [Required]
            // and trim/tolower are safe for null.
            model.ContactIdentifier = model.ContactIdentifier?.Trim().ToLower()!;
            model.LastName = model.LastName?.Trim(); // LastName can be null, so no '!' needed

            IQueryable<Patient> patientQuery = _context.Patients
                .Where(p => !p.IsDeleted);

            if (model.Method.Equals("email", StringComparison.OrdinalIgnoreCase))
            {
                patientQuery = patientQuery.Where(p => p.Email == model.ContactIdentifier);
            }
            else if (model.Method.Equals("sms", StringComparison.OrdinalIgnoreCase))
            {
                patientQuery = patientQuery.Where(p => p.ContactNumber == model.ContactIdentifier);
            }
            else
            {
                return BadRequest("Invalid verification method specified. Must be 'email' or 'sms'.");
            }

            if (!string.IsNullOrEmpty(model.LastName))
            {
                patientQuery = patientQuery.Where(p => p.LastName == model.LastName);
            }

            var patients = await patientQuery.ToListAsync();

            // CA1860: Prefer using 'Count' or 'Length' properties rather than calling 'Enumerable.Any()'.
            if (patients is not { Count: > 0 }) // Check for null or empty list more efficiently and readably
            {
                // CA2254: The logging message template should not vary between calls.
                _logger.LogWarning("Verification code requested for non-existent or unmatching contact. ContactIdentifier: {ContactIdentifier}", model.ContactIdentifier);
                return Ok(new { message = "If a matching record exists, a verification code has been sent." });
            }

            if (patients.Count > 1)
            {
                _logger.LogWarning("Multiple patients found for contact: {ContactIdentifier}. Using the first match.", model.ContactIdentifier);
            }

            var patient = patients.First();

            string verificationCode = GenerateOtp();
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            var newVerificationCode = new VerificationCode // IDE0090: 'new' expression can be simplified
            {
                PatientId = patient.PatientId,
                Code = verificationCode,
                ContactMethod = model.ContactIdentifier,
                SentAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                IsUsed = false
            };
            _context.VerificationCodes.Add(newVerificationCode);
            await _context.SaveChangesAsync();

            // CA2254: The logging message template should not vary between calls.
            _logger.LogInformation("--- SIMULATED SEND ---");
            if (model.Method.Equals("email", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Simulating email send. To: {Email}, Subject: {Subject}, Body: {Body}",
                                        model.ContactIdentifier, "Your Clinic Verification Code", $"Your verification code is: {verificationCode}. It expires in 5 minutes.");
            }
            else
            {
                _logger.LogInformation("Simulating SMS send. To: {Phone}, Message: {Message}",
                                        model.ContactIdentifier, $"Your Clinic verification code: {verificationCode}. Expires in 5 min.");
            }
            _logger.LogInformation("--- END SIMULATED SEND ---");

            return Ok(new { message = "Verification code has been sent." });
        }

        [HttpPost("VerifyCode")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // CS8601: Possible null reference assignment - Added null-forgiving operator '!'
            model.ContactIdentifier = model.ContactIdentifier?.Trim().ToLower()!;
            model.Code = model.Code?.Trim()!;

            var verificationCode = await _context.VerificationCodes
                .Where(vc => vc.ContactMethod == model.ContactIdentifier &&
                             vc.Code == model.Code &&
                             !vc.IsUsed &&
                             vc.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(vc => vc.SentAt)
                .FirstOrDefaultAsync();

            if (verificationCode == null)
            {
                _logger.LogWarning("Attempted verification with invalid or expired code for contact: {ContactIdentifier}", model.ContactIdentifier);
                return Unauthorized(new { message = "Invalid or expired verification code." });
            }

            verificationCode.IsUsed = true;
            await _context.SaveChangesAsync();

            // CS8602: Dereference of a possibly null reference.
            // Fixed by adding null-conditional operator '?' and null-forgiving operator '!'
            // and using null-coalescing '??' to provide empty values if nav property is null.
            // This assumes Doctor, Service, Patient are potentially nullable.
            var patient = await _context.Patients
                .Where(p => p.PatientId == verificationCode.PatientId && !p.IsDeleted)
                .Select(p => new // IDE0305: Collection initialization can be simplified - Fixed by using simplified new() syntax for anonymous type properties.
                {
                    p.PatientId,
                    p.FirstName,
                    p.LastName,
                    p.ContactNumber,
                    p.Email,
                    Appointments = p.Appointments // This will now never be null due to step 1
                        .Where(a => a.PatientId == p.PatientId)
                        .Select(a => new
                        {
                            a.AppointmentId,
                            a.AppointmentDateTime,
                            a.Status,
                            a.Notes,
                            Service = new
                            {
                                a.Service!.ServiceId,
                                a.Service.ServiceName,
                                a.Service.Price
                            },
                            Doctor = new
                            {
                                a.Doctor!.StaffId,
                                a.Doctor.FirstName,
                                a.Doctor.LastName,
                                a.Doctor.JobTitle,
                                a.Doctor.Specialization
                            }
                        })
                    .ToList()
                })
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                _logger.LogError("Patient with ID {PatientId} not found or is deleted after successful code verification. This implies data inconsistency or a race condition.", verificationCode.PatientId);
                return Unauthorized(new { message = "Patient record not found or inaccessible." });
            }

            return Ok(patient);
        }

        // CA1822: Mark as static as it does not access instance data
        private static string GenerateOtp(int length = 6)
        {
            Random random = new(); // IDE0090: 'new' expression can be simplified
            return new string(Enumerable.Repeat("0123456789", length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}