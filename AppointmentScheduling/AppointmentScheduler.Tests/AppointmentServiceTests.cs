using AppointmentScheduler.Domain.DTOs;
using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using AppointmentScheduler.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppointmentScheduler.Tests.Services
{
	public class AppointmentServiceTests
	{
		// better if can do integration tests to test out actual database and define behavior for appointments that bleed to the next day.
		// GetAppointmentsByDoctorAndDateAsync_ShouldReturnExpectedResult - test // Checks if appointments bleed to next day
		[Fact]
		public async Task GetAppointmentSummaryAsync_ShouldReturnCorrectSummary()
		{
			// Arrange
			var mockAppointmentRepository = new Mock<IAppointmentRepository>();
			var startDate = new DateTime(2025, 3, 16);
			var endDate = new DateTime(2025, 3, 17);

			var appointments = new List<Appointment>
		{
			new Appointment { ID = 1, DoctorID = 1, PatientID = 1, StartDate = new DateTime(2025, 3, 16, 9, 0, 0), EndDate = new DateTime(2025, 3, 16, 9, 30, 0), Status = "SCHEDULED" },
			new Appointment { ID = 2, DoctorID = 1, PatientID = 2, StartDate = new DateTime(2025, 3, 16, 10, 0, 0), EndDate = new DateTime(2025, 3, 16, 10, 30, 0), Status = "COMPLETED" },
			new Appointment { ID = 3, DoctorID = 1, PatientID = 3, StartDate = new DateTime(2025, 3, 16, 11, 0, 0), EndDate = new DateTime(2025, 3, 16, 11, 30, 0), Status = "CANCELLED" },
			new Appointment { ID = 4, DoctorID = 1, PatientID = 4, StartDate = new DateTime(2025, 3, 17, 9, 0, 0), EndDate = new DateTime(2025, 3, 17, 9, 30, 0), Status = "SCHEDULED" }
		};

			mockAppointmentRepository.Setup(repo => repo.GetAppointments(startDate, endDate)).ReturnsAsync(appointments);

			var appointmentService = new AppointmentService(mockAppointmentRepository.Object);

			// Act
			var summary = await appointmentService.GetAppointmentSummaryAsync(startDate, endDate);

			// Assert
			Assert.Equal(2, summary.Scheduled.Count);
			Assert.Equal(1, summary.Completed.Count);
			Assert.Equal(1, summary.Cancelled.Count);

			Assert.Contains(appointments[0], summary.Scheduled.Appointments);
			Assert.Contains(appointments[3], summary.Scheduled.Appointments);
			Assert.Contains(appointments[1], summary.Completed.Appointments);
			Assert.Contains(appointments[2], summary.Cancelled.Appointments);
		}
	}
}
