using AutoMapper;
using BusinessLogicLayer.DTO;

namespace BusinessLogicLayer.Mappers
{
    public class UserDTOToOrderResponseMappingProfile : Profile
    {
        public UserDTOToOrderResponseMappingProfile()
        {
            CreateMap<User, OrderResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.PersonName));
        }
    }
}
