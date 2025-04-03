using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentScheduler.Domain.Entities
{
	public class Doctor
	{
		public int ID { get; set; }
		public string? Name { get; set; }
		public string? Specialty { get; set; }

		[NotMapped]
		public List<TimeSlot>? AvailableTimeSlots { get; set; }
	}

	public class TimeSlot
	{
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
