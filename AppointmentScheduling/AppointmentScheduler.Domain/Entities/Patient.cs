namespace AppointmentScheduler.Domain.Entities
{
	public class Patient
	{
		public int ID { get; set; }
		public string? Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? PhoneNumber { get; set; }
		public string? EmailAddress { get; set; }
	}
}