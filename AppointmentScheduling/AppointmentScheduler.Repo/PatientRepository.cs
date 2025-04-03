using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Infrastructure.Repositories
{
	public class PatientRepository : IPatientRepository
	{
		private readonly AppointmentSchedulerDbContext _context;

		public PatientRepository(AppointmentSchedulerDbContext context)
		{
			_context = context;
		}

		public async Task<Patient> GetByIdAsync(int id)
		{
			return await _context.Patients.FindAsync(id);
		}

		// cleanup-hani: delete this?
		public async Task<IEnumerable<Patient>> GetAllAsync()
		{
			return await _context.Patients.ToListAsync();
		}

		// cleanup-hani: delete this?
		public async Task AddAsync(Patient patient)
		{
			await _context.Patients.AddAsync(patient);
			await _context.SaveChangesAsync();
		}

		// cleanup-hani: delete this?
		public async Task UpdateAsync(Patient patient)
		{
			_context.Patients.Update(patient);
			await _context.SaveChangesAsync();
		}

		// cleanup-hani: delete this?
		public async Task DeleteAsync(int id)
		{
			var patient = await _context.Patients.FindAsync(id);
			if (patient != null)
			{
				_context.Patients.Remove(patient);
				await _context.SaveChangesAsync();
			}
		}
	}
}

