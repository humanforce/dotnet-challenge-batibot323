using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Infrastructure;
using AppointmentScheduler.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AppointmentScheduler.Tests
{
	public class AppointmentRepositoryTests
	{
		private DbContextOptions<AppointmentSchedulerDbContext> GetInMemoryDbContextOptions()
		{
			return new DbContextOptionsBuilder<AppointmentSchedulerDbContext>()
				.UseInMemoryDatabase(databaseName: "AppointmentSchedulerTestDb")
				.Options;
		}

		[Theory]
		[InlineData(1, "2025-03-16T09:15:00", "2025-03-16T09:45:00", true)]  // Overlapping
		[InlineData(1, "2025-03-16T09:00:01", "2025-03-16T09:00:02", true)]  // Contained inside
		[InlineData(1, "2025-03-16T08:45:00", "2025-03-16T09:00:01", true)]  // Overlapping in front
		[InlineData(1, "2025-03-16T10:00:00", "2025-03-16T10:30:00", false)] // No overlap
		[InlineData(1, "2025-03-16T09:00:00", "2025-03-16T09:30:00", true)]  // Exact overlap
		[InlineData(1, "2025-03-16T08:30:00", "2025-03-16T09:00:00", false)] // Ends exactly when another starts
		[InlineData(1, "2025-03-16T09:30:00", "2025-03-16T10:00:00", false)] // Starts exactly when another ends
		[InlineData(2, "2025-03-16T09:15:00", "2025-03-16T09:45:00", false)] // Different doctor, overlapping
		[InlineData(2, "2025-03-16T08:45:00", "2025-03-16T09:00:01", false)] // Different doctor, overlapping in front
		[InlineData(2, "2025-03-16T10:00:00", "2025-03-16T10:30:00", false)] // Different doctor, no overlap
		[InlineData(2, "2025-03-16T09:00:00", "2025-03-16T09:30:00", false)] // Different doctor, exact overlap
		[InlineData(2, "2025-03-16T08:30:00", "2025-03-16T09:00:00", false)] // Different doctor, ends exactly when another starts
		[InlineData(2, "2025-03-16T09:30:00", "2025-03-16T10:00:00", false)] // Different doctor, starts exactly when another ends
		public async Task HasConflict_ShouldReturnExpectedResult(
			int newDoctorId, string newStartTime, string newEndTime, bool expectedResult)
		{
			// Arrange
			var options = GetInMemoryDbContextOptions();
			using (var context = new AppointmentSchedulerDbContext(options))
			{
				var repository = new AppointmentRepository(context);
				var existingAppointment = new Appointment
				{
					PatientID = 1,
					DoctorID = 1,
					StartDate = new DateTime(2025, 3, 16, 9, 0, 0),
					EndDate = new DateTime(2025, 3, 16, 9, 30, 0),
					Status = AppointmentStatus.Scheduled
				};
				await context.Appointments.AddAsync(existingAppointment);
				await context.SaveChangesAsync();

				var newAppointment = new Appointment
				{
					PatientID = 2,
					DoctorID = newDoctorId,
					StartDate = DateTime.Parse(newStartTime),
					EndDate = DateTime.Parse(newEndTime),
					Status = AppointmentStatus.Scheduled
				};

				// Act
				var result = await repository.HasConflict(newAppointment);

				// Assert
				Assert.Equal(expectedResult, result);
			}
		}
	}
}