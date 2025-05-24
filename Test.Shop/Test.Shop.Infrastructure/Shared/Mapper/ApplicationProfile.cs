using AutoMapper;
using Test.Shop.Core.Entities;
using Test.Shop.Application.DTO;

namespace Test.Shop.Infrastructure.Shared.Mapper
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<ShopDetails, ShopDetailsDto>().ReverseMap();
            CreateMap<ShopDetails, ShopDetailsAddDto>().ReverseMap();
            CreateMap<ShopDetails, ShopDetailsUpdateDto>().ReverseMap();
        }
    }
}
