using AppointmentScheduler.Domain.Entities;
using System.Collections.Generic;

namespace AppointmentScheduler.API.Dtos
{
    public class PatientAppointmentsDto
    {
        public Patient Patient { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }
    }
}
