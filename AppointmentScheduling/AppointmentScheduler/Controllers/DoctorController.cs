using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AppointmentScheduler.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DoctorController : ControllerBase
	{
		private readonly IDoctorService _doctorService;

		public DoctorController(IDoctorService doctorService)
		{
			_doctorService = doctorService;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetDoctorById(int id)
		{
			var doctor = await _doctorService.GetDoctorByIdAsync(id);
			if (doctor == null)
			{
				return NotFound();
			}
			return Ok(doctor);
		}

		[HttpGet("{doctorId}/available")]
		public async Task<IActionResult> GetAvailableTimeSlots(int doctorId, [FromQuery] DateTime date)
		{
			var doctor = await _doctorService.GetAvailableTimeSlotsAsync(doctorId, date);
			if (doctor == null)
			{
				return NotFound();
			}
			return Ok(doctor);
		}
	}
}
