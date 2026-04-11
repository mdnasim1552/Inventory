using AutoMapper;
using InventoryEntity.Account;
using Inventory.Models;
using InventoryEntity.Brand;
using InventoryEntity.Category;
using InventoryEntity.SubCategory;
using InventoryEntity.Product;
using InventoryEntity.Customer;
using InventoryEntity.Supplier;
using InventoryEntity.Store;
using InventoryEntity.Purchase;

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
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
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
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
              .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
              .ReverseMap();
            CreateMap<ProductViewDto, Product>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
              .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.SubCategoryId))
              .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
              .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
              .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
              .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
              .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
              .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
              .ForPath(dest => dest.Unit.ShortName, opt => opt.MapFrom(src => src.UnitShortName))
              .ForPath(dest => dest.CreatedByNavigation.Role.Role, opt => opt.MapFrom(src => src.Role))
              .ForPath(dest => dest.Category.Name, opt => opt.MapFrom(src => src.CategoryName))
              .ForPath(dest => dest.Brand.Name, opt => opt.MapFrom(src => src.BrandName))
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
            CreateMap<CustomerDto, Customer>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
               .ReverseMap();
            CreateMap<SupplierDto, Supplier>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
               .ReverseMap();
            CreateMap<StoreDto, Store>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ReverseMap();
            CreateMap<PurchaseDto, Purchase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
                .ForMember(dest => dest.InvoiceNo, opt => opt.MapFrom(src => src.InvoiceNo))
                .ForMember(dest => dest.InvoiceFile, opt => opt.MapFrom(src => src.InvoiceFile))
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.PurchaseDate))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ReverseMap();
            CreateMap<PurchaseItemDto, PurchaseItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PurchaseId, opt => opt.MapFrom(src => src.PurchaseId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitCost, opt => opt.MapFrom(src => src.UnitCost))
                .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ReverseMap();

        }
    }
}
