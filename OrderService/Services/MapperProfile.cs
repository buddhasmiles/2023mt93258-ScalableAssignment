using AutoMapper;
using Entities.Dtos;
using Entities.Models;

namespace Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Wish List

            CreateMap<WishListCreateDto, WishList>();
            CreateMap<WishListItemCreateDto, WishListItem>();

            CreateMap<WishList, WishListResponseDto>();
            CreateMap<WishListItem, WishListItemResponseDto>();

            CreateMap<WishListUpdateDto, WishList>();
            CreateMap<WishListItemUpdateDto, WishListItem>();

            // Cart

            CreateMap<AddToCartProductDto, Cart>();

            CreateMap<Cart, CartResponseDto>();

            CreateMap<CartProductUpdateDto, Cart>();

            // Order

            CreateMap<CheckOutFromCartDto, Order>();
            CreateMap<Cart, OrderItem>();

            CreateMap<Order, OrderResponseDto>();
            CreateMap<OrderItem, OrderItemResponseDto>();
        }
    }
}
