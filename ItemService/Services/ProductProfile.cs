using Entities.Dtos;
using Entities.Models;
using AutoMapper;

namespace Services
{
    public class ProductProfile : Profile
    {
        public ProductProfile() 
        {
            CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Convert.FromBase64String(src.Image)));

            CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => Convert.ToBase64String(src.Image)));
        }
    }
}
