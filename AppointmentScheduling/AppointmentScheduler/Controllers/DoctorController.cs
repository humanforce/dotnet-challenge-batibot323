using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

		// cleanup-hani: delete this?
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

		[HttpGet("{doctorId}/available-range")]
		public async Task<IActionResult> GetAvailableTimeSlotsForDateRange(int doctorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var doctor = await _doctorService.GetAvailableTimeSlotsForDateRangeAsync(doctorId, startDate, endDate);
			if (doctor == null)
			{
				return NotFound();
			}
			return Ok(doctor);
		}

		[HttpGet("{doctorId}/appointments")]
		public async Task<IActionResult> GetDoctorAndAppointmentsAsync(int doctorId, [FromQuery] DateTime date)
		{
			var appointments = await _doctorService.GetDoctorAndAppointmentsAsync(doctorId, date);
			if (appointments == null)
			{
				return NotFound();
			}
			return Ok(appointments);
		}
	}
}
