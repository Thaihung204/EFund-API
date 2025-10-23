using EFund_API.Models;
using EFund_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EFund_API.Controllers
{
    [ApiController]
    [Route("api/survey")]
    [Authorize]
    public class CreditSurveyController : ControllerBase
    {
        private readonly ICreditSurveyService _service;

        public CreditSurveyController(ICreditSurveyService service)
        {
            _service = service;
        }

        [HttpPost("")]
        public async Task<ActionResult<CreditSurvey>> Create([FromForm] DataTransferObjects.CreditSurvey dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("score/{id}/{score}")]
        public async Task<ActionResult<CreditSurvey>> UpdateScore(Guid id, int score)
        {
            var updated = await _service.UpdateScore(id, score);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreditSurvey>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreditSurvey>> Get(Guid id)
        {
            var survey = await _service.GetByIdAsync(id);
            if (survey == null) return NotFound();
            return Ok(survey);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<CreditSurvey>> Update(Guid id, [FromBody] CreditSurvey survey)
        {
            var updated = await _service.UpdateAsync(id, survey);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/upload-salaryy")]
        public async Task<IActionResult> CaculateCreditScore(Guid id, IFormFile file)
        {
            var survey = await _service.GetByIdAsync(id);
            if (survey == null) return NotFound();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            survey.SalarySlipImage = ms.ToArray();

            await _service.UpdateAsync(id, survey);
            return Ok("Salary slip uploaded successfully");
        }

        // Upload file bảng lương
        [HttpPost("{id}/upload-salary")]
        public async Task<IActionResult> UploadSalarySlip(Guid id, IFormFile file)
        {
            var survey = await _service.GetByIdAsync(id);
            if (survey == null) return NotFound();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            survey.SalarySlipImage = ms.ToArray();

            await _service.UpdateAsync(id, survey);
            return Ok("Salary slip uploaded successfully");
        }

        // Upload file hóa đơn điện/nước
        [HttpPost("{id}/upload-utility")]
        public async Task<IActionResult> UploadUtilityBill(Guid id, IFormFile file)
        {
            var survey = await _service.GetByIdAsync(id);
            if (survey == null) return NotFound();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            survey.UtilityBillImage = ms.ToArray();

            await _service.UpdateAsync(id, survey);
            return Ok("Utility bill uploaded successfully");
        }
    }
}
