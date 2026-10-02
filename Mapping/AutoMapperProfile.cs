using AutoMapper;
using WebApiLab4.DTOs;
using WebApiLab4.Models;

namespace WebApiLab4.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<ProductInputDto, Product>();
    }
}
