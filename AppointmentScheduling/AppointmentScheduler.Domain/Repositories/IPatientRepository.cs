using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Repositories
{
	public interface IPatientRepository
	{
		Task<Patient> GetByIdAsync(int id);
		Task<IEnumerable<Patient>> GetAllAsync();
		Task AddAsync(Patient patient);
		Task UpdateAsync(Patient patient);
		Task DeleteAsync(int id);
	}
}
