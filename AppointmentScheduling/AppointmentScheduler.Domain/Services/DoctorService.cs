using AppointmentScheduler.Domain.Entities;
using AppointmentScheduler.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	// todo-hani: rename this to a more generic service because this will be too fine-grained.
	public class DoctorService
	{
		private readonly IAppointmentRepository _appointmentRepository;

		public DoctorService(IAppointmentRepository appointmentRepository)
		{
			_appointmentRepository = appointmentRepository;
		}

		public async Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
		{
			var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);

			var takenTimeSlots = appointments.Select(a => new TimeSlot
			{
				StartTime = a.StartTime,
				EndTime = a.EndTime
			}).ToList();

			var availableTimeSlots = ComputeAvailableTimeSlots(takenTimeSlots, date);

			return availableTimeSlots;
		}

		private IEnumerable<TimeSlot> ComputeAvailableTimeSlots(List<TimeSlot> takenTimeSlots, DateTime date)
		{
			var availableTimeSlots = new List<TimeSlot>();

			// Define the working hours (e.g., 9 AM to 5 PM)
			var startOfDay = date.Date.AddHours(9);
			var endOfDay = date.Date.AddHours(17);

			// Add initial available slot from start of day to the first taken slot
			if (takenTimeSlots.Count == 0 || startOfDay < takenTimeSlots.First().StartTime)
			{
				availableTimeSlots.Add(new TimeSlot
				{
					StartTime = startOfDay,
					EndTime = takenTimeSlots.Count > 0 ? takenTimeSlots.First().StartTime : endOfDay
				});
			}

			// Add available slots between taken slots
			for (int i = 0; i < takenTimeSlots.Count - 1; i++)
			{
				var endOfCurrentSlot = takenTimeSlots[i].EndTime;
				var startOfNextSlot = takenTimeSlots[i + 1].StartTime;

				if (endOfCurrentSlot < startOfNextSlot)
				{
					availableTimeSlots.Add(new TimeSlot
					{
						StartTime = endOfCurrentSlot,
						EndTime = startOfNextSlot
					});
				}
			}

			// Add final available slot from the last taken slot to the end of day
			if (takenTimeSlots.Count > 0 && takenTimeSlots.Last().EndTime < endOfDay)
			{
				availableTimeSlots.Add(new TimeSlot
				{
					StartTime = takenTimeSlots.Last().EndTime,
					EndTime = endOfDay
				});
			}

			return availableTimeSlots;
		}
	}
}
