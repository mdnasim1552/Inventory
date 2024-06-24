using AutoMapper;
using InventoryEntity.Account;
using Inventory.Models;
using InventoryEntity.Brand;

namespace Inventory.Profiles
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDto, Credential>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ReverseMap();

            CreateMap<BrandDto, Brand>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
                .ReverseMap(); 
        }
    }
}
