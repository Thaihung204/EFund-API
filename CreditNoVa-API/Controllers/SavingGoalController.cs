using System.Security.Claims;
using EFund_API.Dtos;
using EFund_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EFund_API.Controllers
{
    [ApiController]
    [Route("api/saving-goals")]
    //[Authorize]
    public class SavingGoalController : ControllerBase
    {
        private readonly ISavingGoalService _service;

        public SavingGoalController(ISavingGoalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavingGoalResponse>>> GetAll()
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var goals = await _service.GetAllByUserAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"));
            return Ok(goals);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SavingGoalResponse>> GetById([FromRoute] Guid id)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var goal = await _service.GetByIdAsync(id, new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"));
            if (goal == null) return NotFound();
            return Ok(goal);
        }

        [HttpPost]
        public async Task<ActionResult<SavingGoalResponse>> Create([FromBody] CreateSavingGoalRequest request)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var created = await _service.CreateAsync(new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"), request);
            if (created == null) return BadRequest(new { message = "Invalid saving goal data." });
            return Ok(created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateSavingGoalRequest request)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var success = await _service.UpdateAsync(id, new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"), request);
            if (!success) return NotFound();
            return Ok(new { message = "Updated successfully." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //if (!TryGetUserId(out var userId)) return Unauthorized();
            var success = await _service.DeleteAsync(id, new Guid("B08FE934-4FDE-428D-BDB8-E6E574687459"));
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
