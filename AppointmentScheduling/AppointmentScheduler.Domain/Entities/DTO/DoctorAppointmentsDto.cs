using AppointmentScheduler.Domain.Entities;

namespace AppointmentScheduler.Domain.DTOs
{
	public class DoctorAppointmentsDto
	{
		public Doctor Doctor { get; set; }
		public IEnumerable<Appointment> Appointments { get; set; }
	}
}

