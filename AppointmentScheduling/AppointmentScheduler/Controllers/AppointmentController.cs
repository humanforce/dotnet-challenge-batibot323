using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppointmentScheduler.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AppointmentController : ControllerBase
	{
		private readonly IAppointmentService _appointmentService;

		public AppointmentController(IAppointmentService appointmentService)
		{
			_appointmentService = appointmentService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
		{
			if (appointment == null)
			{
				return BadRequest("Appointment is null.");
			}

			var result = await _appointmentService.CreateAppointment(appointment);
			if (result)
			{
				return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.ID }, appointment);
			}
			else
			{
				return Conflict("Appointment could not be created due to a conflict.");
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetAppointmentById(int id)
		{
			var appointment = await _appointmentService.GetAppointmentById(id);
			if (appointment == null)
			{
				return NotFound();
			}
			return Ok(appointment);
		}
	}
}
