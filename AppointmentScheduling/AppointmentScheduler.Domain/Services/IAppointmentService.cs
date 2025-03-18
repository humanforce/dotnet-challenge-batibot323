using AppointmentScheduler.Domain.DTOs;
using AppointmentScheduler.Domain.Entities;

public interface IAppointmentService
{
	Task<Appointment> GetAppointmentById(int id);
	// returns true if created, otherwise return false.
	Task<bool> CreateAppointment(Appointment appointment);

	Task<bool> UpdateAppointment(Appointment appointment);
	Task<bool> CancelAppointment(int id);
	Task<AppointmentSummaryDto> GetAppointmentSummaryAsync(DateTime startDate, DateTime endDate);
}




