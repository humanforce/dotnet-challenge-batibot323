using AppointmentScheduler.Domain.DTOs;
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
			var status = appointment.Status.ToUpper();
			if (status != AppointmentStatus.Scheduled && status != AppointmentStatus.Completed && status != AppointmentStatus.Cancelled)
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

		// cleanup-hani: delete this?
		public async Task<bool> CancelAppointment(int id)
		{
			try
			{
				await _appointmentRepository.CancelAsync(id);
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

		public async Task<AppointmentSummaryDto> GetAppointmentSummaryAsync(DateTime startDate, DateTime endDate)
		{
			var appointments = await _appointmentRepository.GetAppointments(startDate, endDate);


			var scheduledAppointments = appointments.Where(a => a.Status.ToUpper() == "SCHEDULED").ToList();
			var completedAppointments = appointments.Where(a => a.Status.ToUpper() == "COMPLETED").ToList();
			var cancelledAppointments = appointments.Where(a => a.Status.ToUpper() == "CANCELLED").ToList();
			var summary = new AppointmentSummaryDto
			{
				Scheduled = new AppointmentStatusSummary
				{
					Count = scheduledAppointments.Count,
					Appointments = scheduledAppointments
				},
				Completed = new AppointmentStatusSummary
				{
					Count = completedAppointments.Count,
					Appointments = completedAppointments
				},
				Cancelled = new AppointmentStatusSummary
				{
					Count = cancelledAppointments.Count,
					Appointments = cancelledAppointments
				}
			};

			return summary;
		}
	}
}




