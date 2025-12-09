using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace courses_buynsell_api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HistoryController(IHistoryService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Buyer, Admin")]
    public async Task<IActionResult> GetHistory([FromQuery] CourseQueryParameters courseQueryParameters)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        var result = await service.GetMyHistory(courseQueryParameters, userId);
        return Ok(result);
    }

    [HttpPost("{courseId:int}")]
    [Authorize(Roles = "Buyer, Admin")]
    public async Task<IActionResult> AddHistory(int courseId)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        var result = await service.AddHistoryAsync(userId, courseId);
        if (!result)
        {
            return BadRequest(new { message = "Course not allowed" });
        }

        return StatusCode(201);
    }

    [HttpDelete("clear")]
    [Authorize(Roles = "Buyer, Admin")]
    public async Task<IActionResult> ClearHistory()
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        var result = await service.ClearHistoriesAsync(userId);

        if (!result)
            return NotFound(new { message = "No history to delete." });

        return StatusCode(204);
    }

    [HttpDelete("{courseId:int}")]
    [Authorize(Roles = "Buyer, Admin")]
    public async Task<IActionResult> RemoveHistory(int courseId)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        var result = await service.RemoveHistoryAsync(userId, courseId);
        if (!result)
            return NotFound(new { message = "No history to delete." });
        return StatusCode(204);
    }
}