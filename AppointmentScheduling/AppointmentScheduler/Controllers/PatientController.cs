using AppointmentScheduler.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppointmentScheduler.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet("{patientId}/appointments")]
        public async Task<IActionResult> GetPatientAppointments(int patientId)
        {
            var appointments = await _patientService.GetPatientAppointmentsAsync(patientId);
            if (appointments == null || !appointments.Any())
            {
                return NotFound();
            }
            return Ok(appointments);
        }
    }
}
