using AppointmentScheduler.API.Dtos;
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
			// defer-hani: we can't handle if patientId is int or string here because it gets intercepted.
			var patientAppointments = await _patientService.GetPatientAppointmentsAsync(patientId);
			if (patientAppointments == null)
			{
				return NotFound();
			}
			return Ok(patientAppointments);
		}
	}
}
