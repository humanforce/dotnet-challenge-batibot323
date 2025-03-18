using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public interface IPatientService
	{
		Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId);
	}
}
