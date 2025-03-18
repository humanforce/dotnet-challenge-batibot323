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

        public async Task<bool> HasConflict(Appointment appointment)
		{
			// at first, there's four cases of conflict
			// 1. appointment conflicts at the start
			// 2. appointment conflicts at the end
			// 3. appointment is inside the preexisting one
			// 4. appointment is larger than the preexisting one
			// but actually old.StartDate < new.EndDate actually almost ensures that there's an overlap
            // except if we're way past the preexisting, such that new.StartDate > old.EndDate then no overlap.
			bool hasConflict = await _context.Appointments
				.AnyAsync(a => a.DoctorID == appointment.DoctorID &&
							   a.StartDate < appointment.EndDate &&
							   a.EndDate > appointment.StartDate &&
                               (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Completed));
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
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        // think-hani: look at using deleted timestamp instead of actually deleting in db.
        public async Task DeleteAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }

		public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.DoctorID == doctorId && 
                    (a.StartDate.Date == date.Date || a.EndDate.Date == date.Date))
                .ToListAsync();
		}

		public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
		{
			return await _context.Appointments
				.Where(a => a.PatientID == patientId)
				.ToListAsync();
		}
	}
}
