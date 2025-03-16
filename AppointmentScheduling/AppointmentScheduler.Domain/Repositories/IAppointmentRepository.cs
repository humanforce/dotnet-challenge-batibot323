using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Repositories
{
	public interface IAppointmentRepository
	{
		// todo-hani: refine this. i'd really want to do create appointment by checking for schedule conflicts in the db.
		Task<Appointment> GetByIdAsync(int id);
		Task<IEnumerable<Appointment>> GetAllAsync();
		Task AddAsync(Appointment appointment);
		Task UpdateAsync(Appointment appointment);
		Task DeleteAsync(int id);
		Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);
		Task<bool> HasConflict(Appointment appointment);
	}
}
