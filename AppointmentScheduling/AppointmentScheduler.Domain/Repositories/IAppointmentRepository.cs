using AppointmentScheduler.Domain.DTOs;
using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Repositories
{
	public interface IAppointmentRepository
	{
		Task AddAsync(Appointment appointment);
		Task UpdateAsync(Appointment appointment);
		Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);
		Task<bool> HasConflict(Appointment appointment);
		Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId);
		Task<IEnumerable<Appointment>> GetAppointments(DateTime startDate, DateTime endDate);
		Task CancelAsync(int id);

		// cleanup-hani: delete the ff?
		Task<IEnumerable<Appointment>> GetAllAsync();
		Task<Appointment> GetByIdAsync(int id);
	}
}
