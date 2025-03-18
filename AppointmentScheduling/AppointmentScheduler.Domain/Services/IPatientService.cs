using AppointmentScheduler.API.Dtos;
using AppointmentScheduler.Domain.Entities;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public interface IPatientService
	{
		Task<PatientAppointmentsDto?> GetPatientAppointmentsAsync(int patientId);
	}
}
