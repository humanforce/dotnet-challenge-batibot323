using System.Text.Json.Serialization;

namespace AppointmentScheduler.Domain.Entities
{
	public class Appointment
	{
		public int ID { get; set; }
		public int PatientID { get; set; }
		public int DoctorID { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public String Status { get; set; }

		public bool CanBeScheduled(DateTime proposedTime)
		{
			// Business rule to check if the appointment can be scheduled
			return proposedTime > DateTime.Now;
		}
	}

	public static class AppointmentStatus
	{
		public const string Scheduled = "SCHEDULED";
		public const string Completed = "COMPLETED";
		public const string Cancelled = "CANCELLED";
	}
}
