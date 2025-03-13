using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Repositories
{
	public interface IDoctorRepository
	{
		Task<Doctor> GetByIdAsync(int id);
		Task<IEnumerable<Doctor>> GetAllAsync();
		Task AddAsync(Doctor doctor);
		Task UpdateAsync(Doctor doctor);
		Task DeleteAsync(int id);
	}
}
