namespace AppointmentScheduler.Domain.Entities
{
	public class Appointment
	{
		public int ID { get; set; }
		public int PatientID { get; set; }
		public int DoctorID { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public AppointmentStatus Status { get; set; }

		public bool CanBeScheduled(DateTime proposedTime)
		{
			// Business rule to check if the appointment can be scheduled
			return proposedTime > DateTime.Now;
		}
	}

	public enum AppointmentStatus
	{
		// do we think about pending, especially for doing recurring appointments?
		Scheduled,
		Completed,
		Cancelled
	}
}
