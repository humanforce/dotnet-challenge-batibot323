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
	public class DoctorServiceTests
	{
		private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
		private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
		private readonly DoctorService _doctorService;

		public DoctorServiceTests()
		{
			_appointmentRepositoryMock = new Mock<IAppointmentRepository>();
			_doctorRepositoryMock = new Mock<IDoctorRepository>();
			_doctorService = new DoctorService(_appointmentRepositoryMock.Object, _doctorRepositoryMock.Object);
		}

		[Fact]
		public async Task GetDoctorByIdAsync_ShouldReturnDoctor_WhenDoctorExists()
		{
			// Arrange
			var doctorId = 1;
			var doctor = new Doctor { ID = doctorId, Name = "Dr. Smith", Specialty = "Cardiology" };
			_doctorRepositoryMock.Setup(repo => repo.GetByIdAsync(doctorId)).ReturnsAsync(doctor);

			// Act
			var result = await _doctorService.GetDoctorByIdAsync(doctorId);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(doctorId, result.ID);
		}

		[Fact]
		public async Task GetDoctorByIdAsync_ShouldReturnNull_WhenDoctorDoesNotExist()
		{
			// Arrange
			var doctorId = 1;
			// this
			_doctorRepositoryMock.Setup(repo => repo.GetByIdAsync(doctorId)).ReturnsAsync((Doctor)null);

			// Act
			var result = await _doctorService.GetDoctorByIdAsync(doctorId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task GetAvailableTimeSlotsAsync_ShouldReturnAvailableTimeSlots()
		{
			// Arrange
			var doctorId = 1;
			var date = new DateTime(2023, 10, 15);
			var doctor = new Doctor { ID = doctorId, Name = "Dr. Smith", Specialty = "Cardiology" };

			var appointment1Start = date.AddHours(9);
			var appointment1End = date.AddHours(10);
			var appointment2Start = date.AddHours(11);
			var appointment2End = date.AddHours(12);

			var appointments = new List<Appointment>
			{
				new Appointment { StartDate = appointment1Start, EndDate = appointment1End, Status = AppointmentStatus.Scheduled },
				new Appointment { StartDate = appointment2Start, EndDate = appointment2End, Status = AppointmentStatus.Scheduled }
			};

			_doctorRepositoryMock.Setup(repo => repo.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
			_appointmentRepositoryMock.Setup(repo => repo.GetAppointmentsByDoctorAndDateAsync(doctorId, date)).ReturnsAsync(appointments);

			// Act
			var result = await _doctorService.GetAvailableTimeSlotsAsync(doctorId, date);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(3, result?.AvailableTimeSlots?.Count);

			// Check the available time slots
			Assert.Equal(date.Date, result.AvailableTimeSlots[0].StartTime);
			Assert.Equal(appointment1Start, result.AvailableTimeSlots[0].EndTime);

			Assert.Equal(appointment1End, result.AvailableTimeSlots[1].StartTime);
			Assert.Equal(appointment2Start, result.AvailableTimeSlots[1].EndTime);

			Assert.Equal(appointment2End, result.AvailableTimeSlots[2].StartTime);
			Assert.Equal(date.Date.AddDays(1), result.AvailableTimeSlots[2].EndTime);
		}

		[Fact]
		public async Task GetAvailableTimeSlotsAsync_ShouldReturnNull_WhenDoctorDoesNotExist()
		{
			// Arrange
			var doctorId = 1;
			var date = new DateTime(2023, 10, 15);
			_doctorRepositoryMock.Setup(repo => repo.GetByIdAsync(doctorId)).ReturnsAsync((Doctor)null);

			// Act
			var result = await _doctorService.GetAvailableTimeSlotsAsync(doctorId, date);

			// Assert
			Assert.Null(result);
		}
	}
}
