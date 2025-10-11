using Microsoft.AspNetCore.Mvc;
using Shares.Application.Dtos.BaseClass;
using Shares.Application.IService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Services.ExampleAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ExampleController : ControllerBase
{
    private readonly IExample _exampleService;
    // GET: api/<ExampleController>
    [HttpGet]
    public Task<ResponseDto> Get()
    {
        var rs = _exampleService.ExampleAsync(new RequestDto());
        return rs;
    }
}
