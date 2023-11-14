using Appointment_Scheduler.Model;
using Appointment_Scheduler.RequestPayload;
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Appointment, AppointmentRequest>().ReverseMap();
        CreateMap<Appointment, EditAppointmentRequest>().ReverseMap();
    }

}