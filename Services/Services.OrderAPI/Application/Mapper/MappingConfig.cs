using AutoMapper;
using Services.OrderAPI.Application.Dtos;
using Services.OrderAPI.Domain.Entities;

namespace Services.OrderAPI.Application.Mapper;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Order, OrderDto>().ReverseMap();
            config.CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        });
        return mappingConfig;
    }
}
