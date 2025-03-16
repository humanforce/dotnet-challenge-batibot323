using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public class AppointmentService : IAppointmentService
	{
		private readonly IAppointmentRepository _appointmentRepository;

		public AppointmentService(IAppointmentRepository appointmentRepository)
		{
			_appointmentRepository = appointmentRepository;
		}

		public async Task<bool> CreateAppointment(Appointment appointment)
		{
			var canBeScheduled = !(await _appointmentRepository.HasConflict(appointment));
			if (!canBeScheduled)
			{
				return false;
			}
			try
			{
				await _appointmentRepository.AddAsync(appointment);
			}
			catch (Exception)
			{
				// log here!
				throw;
			}

			return true;
		}

		public async Task<bool> UpdateAppointment(Appointment appointment)
		{
			var canBeScheduled = !(await _appointmentRepository.HasConflict(appointment));
			if (!canBeScheduled)
			{
				return false;
			}
			try
			{
				await _appointmentRepository.UpdateAsync(appointment);
			}
			catch (Exception)
			{
				// log here!
				throw;
			}

			return true;
		}

		public async Task<bool> DeleteAppointment(int id)
		{
			try
			{
				await _appointmentRepository.DeleteAsync(id);
				return true;
			}
			catch (Exception)
			{
				// log here!
				throw;
			}
		}

		public async Task<Appointment> GetAppointmentById(int id)
		{
			return await _appointmentRepository.GetByIdAsync(id);
		}
	}
}




