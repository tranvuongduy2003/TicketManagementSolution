using AutoMapper;
using TicketManagement.Api.Dtos;
using TicketManagement.Api.Models;

namespace TicketManagement.Api;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ApplicationUser, UserDto>().ReverseMap();
            config.CreateMap<ApplicationUser, CustomerDto>().ReverseMap();
            config.CreateMap<ApplicationUser, OrganizerDto>().ReverseMap();
            config.CreateMap<UserDto, OrganizerDto>().ReverseMap();
            config.CreateMap<UserDto, CustomerDto>().ReverseMap();
            config.CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();
            config.CreateMap<Category, CategoryDto>().ReverseMap();
            config.CreateMap<CreateCategoryDto, Category>();
            config.CreateMap<Event, EventDto>().ForMember(edto => edto.CreatorId, e => e.MapFrom(src => src.CreatorId));
            config.CreateMap<CreateEventDto, Event>();
            config.CreateMap<UpdateEventDto, Event>();
            config.CreateMap<AlbumDto, Album>().ReverseMap();
            config.CreateMap<Payment, PaymentDto>().ReverseMap();
            config.CreateMap<CheckoutDto, PaymentDto>()
                .ForMember(dest => dest.Quantity, u => u.MapFrom(src => src.Tickets.Count()))
                .ForMember(dest => dest.CustomerName, u => u.MapFrom(src => src.ContactInfo.FullName))
                .ForMember(dest => dest.CustomerEmail, u => u.MapFrom(src => src.ContactInfo.Email))
                .ForMember(dest => dest.CustomerPhone, u => u.MapFrom(src => src.ContactInfo.PhoneNumber));
            config.CreateMap<PaymentDto, CheckoutDto>();
            config.CreateMap<TicketDto, Ticket>().ReverseMap();
        });
        return mappingConfig;
    }
}