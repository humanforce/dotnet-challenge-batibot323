using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using AppointmentScheduler.API.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public class PatientService : IPatientService
	{
		private readonly IAppointmentRepository _appointmentRepository;
		private readonly IPatientRepository _patientRepository;

		public PatientService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository)
		{
			_appointmentRepository = appointmentRepository;
			_patientRepository = patientRepository;
		}

		public async Task<PatientAppointmentsDto?> GetPatientAppointmentsAsync(int patientId)
		{
			var patient = await _patientRepository.GetByIdAsync(patientId);
			if (patient == null)
			{
				return null;
			}

			var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(patientId);

			// patient history - meaning the appointments that are already completed.
			// we should filter on the repo level but time is of the essence so i just change the filter here depending on what does patient "appointment history" actually mean.
			appointments = appointments.Where(a => a.Status == AppointmentStatus.Completed && a.EndDate < DateTime.Now);

			return new PatientAppointmentsDto
			{
				Patient = patient,
				Appointments = appointments
			};
		}
	}
}
