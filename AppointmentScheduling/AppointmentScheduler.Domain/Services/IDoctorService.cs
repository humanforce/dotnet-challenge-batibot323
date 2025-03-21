﻿using AppointmentScheduler.Domain.DTOs;
using AppointmentScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentScheduler.Domain.Services
{
	public interface IDoctorService
	{
		Task<Doctor> GetDoctorByIdAsync(int id);
		Task<Doctor?> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
		Task<DoctorAppointmentsDto> GetDoctorAndAppointmentsAsync(int doctorId, DateTime date);
		Task<Doctor?> GetAvailableTimeSlotsForDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate);
	}
}
