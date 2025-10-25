using System.Security.Claims;
using EFund_API.Dtos;
using EFund_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFund_API.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    //[Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetByUser()
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var list = await _service.GetByUserAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"));
            return Ok(list);
        }

        [HttpGet("goal/{goalId:guid}")]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetByGoal([FromRoute] Guid goalId)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var list = await _service.GetByGoalAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"), goalId);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionResponse>> GetById([FromRoute] Guid id)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var txn = await _service.GetByIdAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"), id);
            if (txn == null) return NotFound();
            return Ok(txn);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var created = await _service.CreateAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"), request);
            if (created == null) return BadRequest(new { message = "Invalid transaction data or goal mismatch." });
            return Ok(created);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var success = await _service.DeleteAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"), id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully." });
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459");
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim != null && Guid.TryParse(claim, out userId);
        }
    }
}
