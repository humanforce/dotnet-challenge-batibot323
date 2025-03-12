namespace AppointmentScheduler.Domain.Entities
{
	public class Doctor
	{
		public int ID { get; set; }
		public string? Name { get; set; }
		public string? Specialty { get; set; }
		public List<TimeSlot>? AvailableTimeSlots { get; set; }
	}

	public class TimeSlot
	{
		// this wouldn't really have an id supposedly?? but rather, it could be just an index, or maybe can just remove.
		// TimeSlot isn't an Appointment!
		public int ID { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
