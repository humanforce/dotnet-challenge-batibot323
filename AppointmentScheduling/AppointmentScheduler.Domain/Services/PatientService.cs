using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public class PatientService : IPatientService
	{
		private readonly IAppointmentRepository _appointmentRepository;

		public PatientService(IAppointmentRepository appointmentRepository)
		{
			_appointmentRepository = appointmentRepository;
		}

		public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId)
		{
			return await _appointmentRepository.GetAppointmentsByPatientIdAsync(patientId);
		}
	}
}
