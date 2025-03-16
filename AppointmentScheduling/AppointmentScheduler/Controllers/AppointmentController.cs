using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AppointmentScheduler.API.Controllers
{
	// todo-hani: add top-level error handling to uniformize errors.
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
			var (isValid, validationResult) = IsAppointmentValid(appointment);
			if (!isValid && validationResult != null)
			{
				return validationResult;
			}

			try
			{
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
			catch (Exception)
			{
				// log here!
				return StatusCode(500, "An error occurred while creating the appointment.");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateAppointment(int id, [FromBody] Appointment appointment)
		{
			if (id != appointment.ID)
			{
				return BadRequest("Appointment ID mismatch.");
			}

			var (isValid, validationResult) = IsAppointmentValid(appointment);
			if (!isValid && validationResult != null)
			{
				return validationResult;
			}

			try
			{
				var result = await _appointmentService.UpdateAppointment(appointment);
				if (result)
				{
					return NoContent();
				}
				else
				{
					return Conflict("Appointment could not be updated due to a conflict.");
				}
			}
			catch (Exception)
			{
				// log here!
				return StatusCode(500, "An error occurred while updating the appointment.");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAppointment(int id)
		{
			try
			{
				var result = await _appointmentService.DeleteAppointment(id);
				if (result)
				{
					return NoContent();
				}
				else
				{
					return NotFound("Appointment not found.");
				}
			}
			catch (Exception)
			{
				// log here!
				return StatusCode(500, "An error occurred while deleting the appointment.");
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

		private (bool, IActionResult?) IsAppointmentValid(Appointment appointment)
		{
			if (appointment == null)
			{
				return (false, BadRequest("Appointment is null."));
			}

			if (appointment.PatientID <= 0 || appointment.DoctorID <= 0)
			{
				return (false, BadRequest("Invalid PatientID or DoctorID."));
			}

			if (appointment.StartDate == default || appointment.EndDate == default)
			{
				return (false, BadRequest("Invalid StartDate or EndDate."));
			}

			if (appointment.StartDate >= appointment.EndDate)
			{
				return (false, BadRequest("End time must be greater than start time."));
			}

			if (!appointment.CanBeScheduled(appointment.StartDate))
			{
				return (false, BadRequest("Appointment cannot be scheduled in the past."));
			}

			return (true, null);
		}
	}
}




