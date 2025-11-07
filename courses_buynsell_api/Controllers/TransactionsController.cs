using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace courses_buynsell_api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{transactionCode}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDetail(string transactionCode)
    {
        var data = await _service.GetByCodeAsync(transactionCode);
        if (data == null) return NotFound();
        return Ok(data);
    }

    [HttpGet("stats/students")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStudentStats()
    {
        var data = await _service.GetStudentStatsAsync();
        return Ok(data);
    }

    [HttpGet("stats/courses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCourseStats()
    {
        var data = await _service.GetCourseStatsAsync();
        return Ok(data);
    }
}
