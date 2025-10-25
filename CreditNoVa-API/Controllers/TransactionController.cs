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
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var list = await _service.GetByUserAsync(userId);
            return Ok(list);
        }

        [HttpGet("goal/{goalId:guid}")]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetByGoal([FromRoute] Guid goalId)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var list = await _service.GetByGoalAsync(userId, goalId);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TransactionResponse>> GetById([FromRoute] Guid id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var txn = await _service.GetByIdAsync(userId, id);
            if (txn == null) return NotFound();
            return Ok(txn);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var created = await _service.CreateAsync(userId, request);
            if (created == null) return BadRequest(new { message = "Invalid transaction data or goal mismatch." });
            return Ok(created);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var success = await _service.DeleteAsync(userId, id);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully." });
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim != null && Guid.TryParse(claim, out userId);
        }
    }
}
