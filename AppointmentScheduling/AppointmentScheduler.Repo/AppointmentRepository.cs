using AppointmentScheduler.Domain.DTOs;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// nit-hani: can rename namespace?
namespace AppointmentScheduler.Infrastructure.Repositories
{
	public class AppointmentRepository : IAppointmentRepository
	{
		private readonly AppointmentSchedulerDbContext _context;

		public AppointmentRepository(AppointmentSchedulerDbContext context)
		{
			_context = context;
		}

		public async Task<bool> HasConflict(Appointment newAppointment)
		{
			// at first, there's four cases of conflict
			// 1. appointment conflicts at the start
			// 2. appointment conflicts at the end
			// 3. appointment is inside the preexisting one
			// 4. appointment is larger than the preexisting one
			// but actually old.StartDate < new.EndDate actually almost ensures that there's an overlap
			// except if we're way past the preexisting, such that new.StartDate > old.EndDate then no overlap.
			bool hasConflict = await _context.Appointments
				.AnyAsync(existing => existing.ID != newAppointment.ID &&
								(existing.DoctorID == newAppointment.DoctorID || existing.PatientID == newAppointment.PatientID) &&
								existing.StartDate < newAppointment.EndDate &&
								existing.EndDate > newAppointment.StartDate &&
								(existing.Status == AppointmentStatus.Scheduled || existing.Status == AppointmentStatus.Completed));
			return hasConflict;
		}

		public async Task<Appointment> GetByIdAsync(int id)
		{
			return await _context.Appointments.FindAsync(id);
		}

		public async Task<IEnumerable<Appointment>> GetAllAsync()
		{
			return await _context.Appointments.ToListAsync();
		}

		public async Task AddAsync(Appointment appointment)
		{
			appointment.Status = appointment.Status.ToUpper();
			await _context.Appointments.AddAsync(appointment);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Appointment appointment)
		{
			_context.Appointments.Update(appointment);
			await _context.SaveChangesAsync();
		}

		public async Task CancelAsync(int id)
		{
			var appointment = await _context.Appointments.FindAsync(id);
			if (appointment != null)
			{
				appointment.Status = "CANCELLED";
				_context.Appointments.Update(appointment);
				await _context.SaveChangesAsync();
			}
		}

		// 
		// Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime dateStart, DateTime? dateEnd = null);

		public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime dateStart, DateTime? dateEnd = null)
		{
			dateEnd ??= dateStart; // If dateEnd is null, set it to dateStart

			return await _context.Appointments
				.Where(a => a.DoctorID == doctorId &&
							(a.StartDate.Date >= dateStart.Date || a.EndDate.Date >= dateStart.Date) &&
							(a.EndDate.Date <= dateEnd.Value.Date || a.StartDate.Date <= dateEnd.Value.Date))
				.ToListAsync();
		}

		public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
		{
			return await _context.Appointments
				.Where(a => a.PatientID == patientId)
				.OrderByDescending(a => a.StartDate)
				.ToListAsync();
		}

		public async Task<IEnumerable<Appointment>> GetAppointments(DateTime startDate, DateTime endDate)
		{
			var appointments = await _context.Appointments
				// this means that appointments have to be contained within the date range.
				// what about those that started at endDate 23:59 and ended at endDate+1 00:01?
				// .Where(a => a.StartDate < endDate && a.EndDate > startDate)
				.Where(a => a.StartDate >= startDate && a.EndDate <= endDate)
				.ToListAsync();
			return appointments;
		}
	}
}
