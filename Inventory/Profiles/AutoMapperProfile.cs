using AutoMapper;
using InventoryEntity.Account;
using Inventory.Models;
using InventoryEntity.Brand;
using InventoryEntity.Category;
using InventoryEntity.SubCategory;
using InventoryEntity.Product;

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
            CreateMap<UserDto, Credential>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.ConfirmPassword))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src =>Convert.ToDateTime(src.Birthday)))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile))
            .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Designation))
            .ReverseMap();

            CreateMap<ProductDto, Product>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
              .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.SubCategoryId))
              .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
              .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
              .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
              .ForMember(dest => dest.MinQuantity, opt => opt.MapFrom(src => src.MinQuantity))
              .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
              .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
              .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
              .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
              .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
              .ReverseMap();

            CreateMap<BrandDto, Brand>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
                .ReverseMap();

            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ReverseMap();

            CreateMap<SubCategoryDto, SubCategory>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
               .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
               .ReverseMap();
        }
    }
}
