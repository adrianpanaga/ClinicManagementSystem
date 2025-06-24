using ClinicManagement.ApiNew.DTOs.util;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClinicManagement.ApiNew.DTOs.Appointments
{
    public class AvailabilityRequestDto
    {
        [Required]
        public int ServiceId { get; set; }
        public int? DoctorId { get; set; } // Nullable if doctor selection is optional

        [Required]
        [DataType(DataType.Date)]
        [JsonConverter(typeof(DateOnlyJsonConverter))] // Ensure correct date parsing if using DateOnly
        public DateOnly Date { get; set; } // Or DateTime if you prefer
    }
}
