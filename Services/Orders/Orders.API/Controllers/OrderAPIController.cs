using Microsoft.AspNetCore.Mvc;
using Orders.Application.Dtos;
using Orders.Application.IServices;
using Orders.Domain.Enums;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Orders.API.Controllers;
[Route("api/order")]
[ApiController]
public class OrderAPIController : ControllerBase
{
    private readonly IOrderService _orderService;
    public OrderAPIController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var rs = await _orderService.GetById(Guid.Parse(id));
        if (!rs.IsSuccess)
            return BadRequest(rs);
        return Ok(rs);
    }

    [HttpGet("get/tracking/{id}")]
    public async Task<IActionResult> GetByTracking(string id)
    {
        var rs = await _orderService.GetByTrackingId(Guid.Parse(id));
        if (!rs.IsSuccess)
            return BadRequest(rs);
        return Ok(rs);
    }

    // POST api/<OrderAPIController>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
    {
        var rs = await _orderService.Save(orderDto);
        if (!rs.IsSuccess)
            return BadRequest(rs);
        return Ok(rs);
    }

    
    [HttpPut("approve")]
    public async Task<IActionResult> ApproveOrder([FromBody] OrderDto orderDto)
    {
        
        var updatedOrderDto = orderDto with { OrderStatus = OrderStatus.Approved.ToString() };
        var rs = await _orderService.Update(updatedOrderDto);
        if (!rs.IsSuccess)
            return BadRequest(rs);

        return Ok(rs);
    }

    [HttpPut("paid")]
    public async Task<IActionResult> PayOrder([FromBody] string orderId)
    {
        var orderRs = await _orderService.GetById(Guid.Parse(orderId));
        if (!orderRs.IsSuccess || orderRs.Data == null)
            return BadRequest(orderRs);

        var updatedOrderDto = orderRs.Data with { OrderStatus = OrderStatus.Paid.ToString() };
        var rs = await _orderService.Update(updatedOrderDto);
        if (!rs.IsSuccess)
            return BadRequest(rs);

        return Ok(rs);
    }
}
