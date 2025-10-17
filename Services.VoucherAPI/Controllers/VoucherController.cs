using Microsoft.AspNetCore.Mvc;
using MediatR;
using Services.VoucherAPI.CQRS.Commands;
using Services.VoucherAPI.CQRS.Queries;
using Services.VoucherAPI.Models;

namespace Services.VoucherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoucherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 🟢 Lấy tất cả vouchers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetVouchersQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // 🟣 Tạo voucher mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVoucherCommand cmd)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetAll), new { id = result.VoucherId }, result);
        }

        // 🟡 Tăng lượt sử dụng
        [HttpPut("{id}/use")]
        public async Task<IActionResult> IncreaseUsage(Guid id)
        {
            var result = await _mediator.Send(new IncreaseUsageCommand(id));
            return Ok(result);
        }

        // 🔵 Cập nhật trạng thái
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            var result = await _mediator.Send(new UpdateStatusCommand(id, status));
            return Ok(result);
        }
    }
}
