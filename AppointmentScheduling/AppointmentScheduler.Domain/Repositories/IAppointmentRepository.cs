using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Repositories
{
	public interface IAppointmentRepository
	{
		Task<Appointment> GetByIdAsync(int id);
		Task<IEnumerable<Appointment>> GetAllAsync();
		Task AddAsync(Appointment appointment);
		Task UpdateAsync(Appointment appointment);
		Task DeleteAsync(int id);
		Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);
		Task<bool> HasConflict(Appointment appointment);
		Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
	}
}
