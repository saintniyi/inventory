using AutoMapper;
using InventoryModel.Models;
using InventoryDto;


namespace InventoryAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<Product, ProductReadDto>();
            CreateMap<ProductWriteDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.ProductImage, opt => opt.Ignore());

            // Supplier mappings
            CreateMap<Supplier, SupplierReadDto>();
            CreateMap<SupplierWriteDto, Supplier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

        }

    }
}
