using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.OrderAPI.Application.Dtos;
using Services.OrderAPI.Application.IServices;
using Services.OrderAPI.Domain.Entities;
using Services.OrderAPI.Domain.Enum;
using Services.OrderAPI.Infrastructure.DataBase;
using Shares.Base.BaseClass;

namespace Services.OrderAPI.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly OrderDbContext _db;
    public OrderService(IMapper mapper, OrderDbContext orderDbContext)
    {
        _mapper = mapper;
        _db = orderDbContext;
    }

    public async Task<ResultService<OrderDto>> GetById(Guid id)
    {
        var rs = new ResultService<OrderDto>();
        try
        {
            var entity = _mapper.Map<OrderDto>(await _db.Orders.FindAsync(id));
            if (entity == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found";
                return rs;
            }
            rs.Data = entity;
            rs.IsSuccess = true;


        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = ex.Message;
        }
        return rs;
    }

    public async Task<ResultService<OrderDto>> GetByTrackingId(Guid trackingId)
    {
       var rs = new ResultService<OrderDto>();
        try
        {
            var entity = _mapper.Map<OrderDto>(await _db.Orders.FirstOrDefaultAsync(o => o.TrackingId == trackingId));
            if (entity == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found";
                return rs;
            }
            rs.Data = entity;
            rs.IsSuccess = true;
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = ex.Message;
        }
        return rs;
    }

    public async Task<ResultService<OrderDto>> Save(OrderDto order)
    {
        var rs = new ResultService<OrderDto>();
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                var entity = _mapper.Map<Order>(order);
                var items = _mapper.Map<IEnumerable<OrderItem>>(order.Items);
                entity.CreatedAt = DateTimeOffset.UtcNow;
                entity.OrderStatus = OrderStatus.Pending.ToString();
                await _db.Orders.AddAsync(entity);
                await _db.SaveChangesAsync();
                foreach (var item in items)
                {
                    item.OrderId = entity.Id;
                }
                await _db.OrderItems.AddRangeAsync(items);
                await _db.SaveChangesAsync();
                transaction.Commit();
                rs.Data = _mapper.Map<OrderDto>(entity);
                rs.Data.Items = _mapper.Map<IEnumerable<OrderItemDto>>(items);
                rs.IsSuccess = true;
                rs.Message = "Order created successfully";

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                rs.IsSuccess = false;
                rs.Message = ex.Message;
            }
        }
        return rs;
    }

    public Task<ResultService<OrderDto>> Update(OrderDto order)
    {
        throw new NotImplementedException();
    }
}
