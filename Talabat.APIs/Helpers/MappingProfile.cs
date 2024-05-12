using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(d => d.Brand, O => O.MapFrom(s => s.Brand))
                .ForMember(d => d.Category, O => O.MapFrom(s => s.Category))
                //With Resolving use generic mapfrom
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<AddressDto , Core.Entities.OrderAggregate.Address>();
            CreateMap<Order,OrderToReturnDto>()
                .ForMember(d=>d.DeliveryMethod ,options=>options.MapFrom(s=>s.DeliveryMethod.ShortName))
                .ForMember(d=>d.DeliveryMethodCost ,options=>options.MapFrom(s=>s.DeliveryMethod.Cost))
                ;
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, options => options.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, options => options.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, options => options.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, options => options.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
