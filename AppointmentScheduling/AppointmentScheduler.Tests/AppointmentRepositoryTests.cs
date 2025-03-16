using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Infrastructure;
using AppointmentScheduler.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
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
		[InlineData(1, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Scheduled, true)]  // Overlapping
		[InlineData(1, "2025-03-16T09:00:01", "2025-03-16T09:00:02", AppointmentStatus.Scheduled, true)]  // Contained inside
		[InlineData(1, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Scheduled, true)]  // Overlapping in front
		[InlineData(1, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Scheduled, false)] // No overlap
		[InlineData(1, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Scheduled, true)]  // Exact overlap
		[InlineData(1, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Scheduled, false)] // Ends exactly when another starts
		[InlineData(1, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Scheduled, false)] // Starts exactly when another ends

		// different doctor should have no conflicts
		[InlineData(2, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Scheduled, false)] // Different doctor, overlapping
		[InlineData(2, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Scheduled, false)] // Different doctor, overlapping in front
		[InlineData(2, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Scheduled, false)] // Different doctor, no overlap
		[InlineData(2, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Scheduled, false)] // Different doctor, exact overlap
		[InlineData(2, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Scheduled, false)] // Different doctor, ends exactly when another starts
		[InlineData(2, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Scheduled, false)] // Different doctor, starts exactly when another ends

		// cancelled status should return no conflict whether there's overlap or none.
		[InlineData(1, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Cancelled, true)] // Overlapping
		[InlineData(1, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Cancelled, true)] // Overlapping in front
		[InlineData(1, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Cancelled, false)] // No overlap
		[InlineData(1, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Cancelled, true)] // Exact overlap
		[InlineData(1, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Cancelled, false)] // Ends exactly when another starts
		[InlineData(1, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Cancelled, false)] // Starts exactly when another ends
		[InlineData(2, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Cancelled, false)] // Different doctor, overlapping
		[InlineData(2, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Cancelled, false)] // Different doctor, overlapping in front
		[InlineData(2, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Cancelled, false)] // Different doctor, no overlap
		[InlineData(2, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Cancelled, false)] // Different doctor, exact overlap
		[InlineData(2, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Cancelled, false)] // Different doctor, ends exactly when another starts
		[InlineData(2, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Cancelled, false)] // Different doctor, starts exactly when another ends

		// not sure what to do with completed status, do we have guarantees that appointments will be marked as completed only when the appointment end time has lapsed?
		// let's just treat completed as scheduled in deciding conflicts.
		[InlineData(1, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Completed, true)]  // Overlapping
		[InlineData(1, "2025-03-16T09:00:01", "2025-03-16T09:00:02", AppointmentStatus.Completed, true)]  // Contained inside
		[InlineData(1, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Completed, true)]  // Overlapping in front
		[InlineData(1, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Completed, false)] // No overlap
		[InlineData(1, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Completed, true)]  // Exact overlap
		[InlineData(1, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Completed, false)] // Ends exactly when another starts
		[InlineData(1, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Completed, false)] // Starts exactly when another ends
		[InlineData(2, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Completed, false)] // Different doctor, overlapping
		[InlineData(2, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Completed, false)] // Different doctor, overlapping in front
		[InlineData(2, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Completed, false)] // Different doctor, no overlap
		[InlineData(2, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Completed, false)] // Different doctor, exact overlap
		[InlineData(2, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Completed, false)] // Different doctor, ends exactly when another starts
		[InlineData(2, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Completed, false)] // Different doctor, starts exactly when another ends
		public async Task HasConflict_ShouldReturnExpectedResult(
			int newDoctorId, string newStartTime, string newEndTime, string status, bool expectedResult)
		{
			// Arrange
			var options = GetInMemoryDbContextOptions();
			using (var context = new AppointmentSchedulerDbContext(options))
			{
				context.Database.EnsureDeleted(); // Reset the database
				context.Database.EnsureCreated(); // Recreate the database

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
					Status = status
				};

				// Act
				var result = await repository.HasConflict(newAppointment);

				// Assert
				Assert.Equal(expectedResult, result);
			}
		}

		[Theory]
		[InlineData(1, "2025-03-16T09:15:00", "2025-03-16T09:45:00", AppointmentStatus.Scheduled, false)]  // Overlapping
		[InlineData(1, "2025-03-16T09:00:01", "2025-03-16T09:00:02", AppointmentStatus.Scheduled, false)]  // Contained inside
		[InlineData(1, "2025-03-16T08:45:00", "2025-03-16T09:00:01", AppointmentStatus.Scheduled, false)]  // Overlapping in front
		[InlineData(1, "2025-03-16T10:00:00", "2025-03-16T10:30:00", AppointmentStatus.Scheduled, false)] // No overlap
		[InlineData(1, "2025-03-16T09:00:00", "2025-03-16T09:30:00", AppointmentStatus.Scheduled, false)]  // Exact overlap
		[InlineData(1, "2025-03-16T08:30:00", "2025-03-16T09:00:00", AppointmentStatus.Scheduled, false)] // Ends exactly when another starts
		[InlineData(1, "2025-03-16T09:30:00", "2025-03-16T10:00:00", AppointmentStatus.Scheduled, false)] // Starts exactly when another ends
		public async Task HasConflict_WithCancelledExistingAppointment_ShouldReturnExpectedResult(
			int newDoctorId, string newStartTime, string newEndTime, string status, bool expectedResult)
		{
			// Arrange
			var options = GetInMemoryDbContextOptions();
			using (var context = new AppointmentSchedulerDbContext(options))
			{
				context.Database.EnsureDeleted(); // Reset the database
				context.Database.EnsureCreated(); // Recreate the database

				var repository = new AppointmentRepository(context);
				var existingAppointment = new Appointment
				{
					PatientID = 1,
					DoctorID = 1,
					StartDate = new DateTime(2025, 3, 16, 9, 0, 0),
					EndDate = new DateTime(2025, 3, 16, 9, 30, 0),
					Status = AppointmentStatus.Cancelled
				};
				await context.Appointments.AddAsync(existingAppointment);
				await context.SaveChangesAsync();

				var newAppointment = new Appointment
				{
					PatientID = 2,
					DoctorID = newDoctorId,
					StartDate = DateTime.Parse(newStartTime),
					EndDate = DateTime.Parse(newEndTime),
					Status = status
				};

				// Act
				var result = await repository.HasConflict(newAppointment);

				// Assert
				Assert.Equal(expectedResult, result);
			}
		}
	}
}
