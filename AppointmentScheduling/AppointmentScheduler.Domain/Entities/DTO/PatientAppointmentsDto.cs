using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;

namespace AppointmentScheduler.Domain.DTOs
{
    public class PatientAppointmentsDto
    {
        public Patient Patient { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
    }
}
