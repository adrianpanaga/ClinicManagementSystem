// Location: C:\Users\AdrianPanaga\NewClinicApi\ClinicManagement.ApiNew\Controllers\ServicesController.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // For authorization attributes

using ClinicManagement.Data.Context; // Your DbContext
using ClinicManagement.Data.Models;   // Your EF Core models
using ClinicManagement.ApiNew.DTOs.Services; // Your Service DTOs

namespace ClinicManagement.ApiNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All actions in this controller require an authenticated user by default
    public class ServicesController : ControllerBase
    {
        private readonly ClinicManagementDbContext _context;

        public ServicesController(ClinicManagementDbContext context)
        {
            _context = context;
        }

        // Helper method to map Service model to ServiceDto
        private ServiceDto MapToServiceDto(Service service)
        {
            return new ServiceDto
            {
                ServiceId = service.ServiceId,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt
            };
        }

        // GET: api/Services
        /// <summary>
        /// Retrieves all services. Accessible by all authenticated users.
        /// </summary>
        /// <returns>A list of ServiceDto.</returns>
        [HttpGet]
        [AllowAnonymous] // Services can be viewed by anyone, even unauthenticated users (if public facing)
                         // Or, if only authenticated users should see them, remove [AllowAnonymous]
                         // For clinic internal use, [Authorize] on class level is sufficient.
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            if (_context.Services == null)
            {
                return NotFound();
            }

            var services = await _context.Services.ToListAsync();
            return services.Select(s => MapToServiceDto(s)).ToList();
        }

        // GET: api/Services/5
        /// <summary>
        /// Retrieves a specific service by its ID. Accessible by all authenticated users.
        /// </summary>
        /// <param name="id">The ID of the service.</param>
        /// <returns>The ServiceDto if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous] // Same logic as GetServices()
        public async Task<ActionResult<ServiceDto>> GetService(int id)
        {
            if (_context.Services == null)
            {
                return NotFound();
            }
            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return MapToServiceDto(service);
        }

        // PUT: api/Services/5
        /// <summary>
        /// Updates an existing service.
        /// Requires Admin or Receptionist role.
        /// </summary>
        /// <param name="id">The ID of the service to update.</param>
        /// <param name="updateServiceDto">The DTO object with updated data.</param>
        /// <returns>NoContent if successful, BadRequest if ID mismatch, NotFound if service not found, or throws exception for concurrency.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Receptionist")] // Only Admin or Receptionist can update services
        public async Task<IActionResult> PutService(int id, UpdateServiceDto updateServiceDto)
        {
            if (id != updateServiceDto.ServiceId)
            {
                return BadRequest("Mismatched Service ID in route and body.");
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            // Manually map properties from DTO to the existing EF Core entity
            service.ServiceName = updateServiceDto.ServiceName!; // Added null-forgiving operator
            service.Description = updateServiceDto.Description!; // Added null-forgiving operator
            service.Price = updateServiceDto.Price;
            service.UpdatedAt = DateTime.UtcNow; // Set update timestamp

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // POST: api/Services
        /// <summary>
        /// Creates a new service.
        /// Requires Admin or Receptionist role.
        /// </summary>
        /// <param name="createServiceDto">The DTO object to create.</param>
        /// <returns>The created ServiceDto with its ID, or a problem if the DbSet is null.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")] // Only Admin or Receptionist can create services
        public async Task<ActionResult<ServiceDto>> PostService(CreateServiceDto createServiceDto)
        {
            if (_context.Services == null)
            {
                return Problem("Entity set 'ClinicManagementDbContext.Services' is null.");
            }

            // Manually map DTO to EF Core model
            var service = new Service
            {
                ServiceName = createServiceDto.ServiceName!, // Added null-forgiving operator
                Description = createServiceDto.Description!, // Added null-forgiving operator
                Price = createServiceDto.Price,
                CreatedAt = DateTime.UtcNow, // Set creation timestamp
                UpdatedAt = DateTime.UtcNow  // Set initial update timestamp
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetService", new { id = service.ServiceId }, MapToServiceDto(service));
        }

        // DELETE: api/Services/5
        /// <summary>
        /// Deletes a service by its ID.
        /// Requires Admin role.
        /// </summary>
        /// <param name="id">The ID of the service to delete.</param>
        /// <returns>NoContent if successful, or NotFound if the service does not exist.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete services (consider soft delete if applicable)
        public async Task<IActionResult> DeleteService(int id)
        {
            if (_context.Services == null)
            {
                return NotFound();
            }
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent(); // Indicate successful deletion
        }

        private bool ServiceExists(int id)
        {
            return (_context.Services?.Any(e => e.ServiceId == id)).GetValueOrDefault();
        }
    }
}
