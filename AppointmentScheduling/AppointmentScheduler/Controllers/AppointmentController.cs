﻿using AppointmentScheduler.Domain.Entities;
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
			if (appointment == null)
			{
				return BadRequest("Appointment is null.");
			}

			if (appointment.PatientID <= 0 || appointment.DoctorID <= 0)
			{
				return BadRequest("Invalid PatientID or DoctorID.");
			}

			if (appointment.StartDate == default || appointment.EndDate == default)
			{
				return BadRequest("Invalid StartDate or EndDate.");
			}

			if (appointment.StartDate >= appointment.EndDate)
			{
				return BadRequest("End time must be greater than start time.");
			}

			if (!appointment.CanBeScheduled(appointment.StartDate))
			{
				return BadRequest("Appointment cannot be scheduled in the past.");
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
