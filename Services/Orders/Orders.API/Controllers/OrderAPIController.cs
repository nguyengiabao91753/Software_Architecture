using Microsoft.AspNetCore.Mvc;
using Orders.Application.Dtos;
using Orders.Application.IServices;


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
    // GET: api/<OrderAPIController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    // GET api/<OrderAPIController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
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

    // PUT api/<OrderAPIController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<OrderAPIController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
