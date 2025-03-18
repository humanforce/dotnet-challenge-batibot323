using AppointmentScheduler.Domain.DTOs;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public class DoctorService : IDoctorService
	{
		private readonly IAppointmentRepository _appointmentRepository;
		private readonly IDoctorRepository _doctorRepository;

		public DoctorService(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository)
		{
			_appointmentRepository = appointmentRepository;
			_doctorRepository = doctorRepository;
		}

		public async Task<Doctor> GetDoctorByIdAsync(int id)
		{
			return await _doctorRepository.GetByIdAsync(id);
		}

		public async Task<Doctor?> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
		{
			var doctor = await _doctorRepository.GetByIdAsync(doctorId);
			if (doctor == null)
			{
				return null;
			}

			var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);
			doctor.AvailableTimeSlots = CalculateAvailableTimeSlots(appointments, date).ToList();

			return doctor;
		}

		public async Task<DoctorAppointmentsDto> GetDoctorAndAppointmentsAsync(int doctorId, DateTime date)
		{
			var doctor = await _doctorRepository.GetByIdAsync(doctorId);
			if (doctor == null)
			{
				return null;
			}

			var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);
			// this can be ignored but it just makes sense to add. you can remove this but also update dto so it doesn't include available time slots.
			doctor.AvailableTimeSlots = CalculateAvailableTimeSlots(appointments, date).ToList();
			var doctorAppointments = new DoctorAppointmentsDto { Doctor = doctor, Appointments = appointments };

			return doctorAppointments;
		}

		private IEnumerable<TimeSlot> CalculateAvailableTimeSlots(IEnumerable<Appointment> appointments, DateTime date)
		{
			var timeSlots = new List<TimeSlot>();

			// Sort appointments by start time
			var sortedAppointments = appointments
				.Where(a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Completed)
				.OrderBy(a => a.StartDate).ToList();

			// Initialize the start of the day to midnight
			var currentStart = date.Date;

			// creates timeslots where t.start is the previous appointment, a.end and t.end is the next a.start.
			foreach (var appointment in sortedAppointments)
			{
				if (appointment.StartDate > currentStart)
				{
					timeSlots.Add(new TimeSlot { StartTime = currentStart, EndTime = appointment.StartDate });
				}
				currentStart = appointment.EndDate > currentStart ? appointment.EndDate : currentStart;
			}

			// Add the remaining time slot until the end of the day
			var endOfDay = date.Date.AddDays(1);
			if (currentStart < endOfDay)
			{
				timeSlots.Add(new TimeSlot { StartTime = currentStart, EndTime = endOfDay });
			}

			return timeSlots;
		}
	}
}
