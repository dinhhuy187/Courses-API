using System.Security.Claims;
using CloudinaryDotNet.Actions;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace courses_buynsell_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAnonymously([FromQuery] CourseQueryParameters queryParameters)
        {
            if (((queryParameters.IncludeRestricted ?? false) || (queryParameters.IncludeUnapproved ?? false)))
                return BadRequest();
            var result = await courseService.GetCoursesAsync(queryParameters);
            return Ok(result);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin, Buyer, Seller")]
        public async Task<IActionResult> Get([FromQuery] CourseQueryParameters queryParameters)
        {
            if (((queryParameters.IncludeRestricted ?? false) || (queryParameters.IncludeUnapproved ?? false)) 
                && User.IsInRole("Buyer"))
                return BadRequest("Buyer can not get restricted or unapproved courses");
            var result = await courseService.GetCoursesAsync(queryParameters);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await courseService.GetByIdAsync(id,!(User.IsInRole("Admin")||User.IsInRole("Seller")));
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpGet("student/{courseId:int}")]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> GetCourseStudents(int courseId)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var isAdmin = User.IsInRole("Admin");
            var result = await courseService.GetCourseStudents(courseId, userId, isAdmin);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Create([FromForm] CreateCourseDto createCourseDto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var created = await courseService.CreateAsync(createCourseDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateCourseDto updateCourseDto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var updated = await courseService.UpdateAsync(id, updateCourseDto, userId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await courseService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{courseId:int}/contents")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> AddContent(int courseId, [FromBody] CourseContentDto dto)
        {
            var result = await courseService.AddCourseContentAsync(courseId, dto);
            return CreatedAtAction(nameof(GetById), new { id = courseId }, result);
        }

        [HttpDelete("{courseId:int}/contents/{contentId:int}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> RemoveContent(int courseId, int contentId)
        {
            var ok = await courseService.RemoveCourseContentAsync(courseId, contentId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{courseId:int}/skills")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> AddSkill(int courseId, [FromBody] SkillTargetDto dto)
        {
            var result = await courseService.AddCourseSkillAsync(courseId, dto);
            return CreatedAtAction(nameof(GetById), new { id = courseId }, result);
        }

        [HttpPut("{courseId:int}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int courseId)
        {
            await courseService.ApproveCourse(courseId);
            return NoContent();
        }
        
        [HttpPut("{courseId:int}/restrict")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restrict(int courseId)
        {
            var result = await courseService.RestrictCourse(courseId);
            return Ok(result);
        }

        [HttpDelete("{courseId:int}/skills/{skillId:int}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> RemoveSkill(int courseId, int skillId)
        {
            var ok = await courseService.RemoveCourseSkillAsync(courseId, skillId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{courseId:int}/target-learners")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> AddTargetLearner(int courseId, [FromBody] SkillTargetDto dto)
        {
            var result = await courseService.AddTargetLearnerAsync(courseId, dto);
            return CreatedAtAction(nameof(GetById), new { id = courseId }, result);
        }

        [HttpDelete("{courseId:int}/target-learners/{learnerId:int}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> RemoveTargetLearner(int courseId, int learnerId)
        {
            var ok = await courseService.RemoveTargetLearnerAsync(courseId, learnerId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}