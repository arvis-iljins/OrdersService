using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Mappers
{
    public class OrderToOrderResponseMappingProfile : Profile
    {
        public OrderToOrderResponseMappingProfile()
        {
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.UserPersonName));
        }
    }
}
