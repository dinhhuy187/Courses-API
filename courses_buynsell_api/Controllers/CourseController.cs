using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet]
        [Authorize(Roles = "Admin, Buyer, Seller")]
        public async Task<IActionResult> Get([FromQuery] CourseQueryParameters queryParameters)
        {
            var result = await courseService.GetCoursesAsync(queryParameters);
            return Ok(result);
        }
        
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin, Buyer, Seller")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await courseService.GetByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Create([FromForm] CreateCourseDto createCourseDto)
        {
            var created = await courseService.CreateAsync(createCourseDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateCourseDto updateCourseDto)
        {
            var updated = await courseService.UpdateAsync(id, updateCourseDto);
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
        public async Task<IActionResult> AddContent(int courseId, [FromBody] ContentSkillTargetDto dto)
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
        public async Task<IActionResult> AddSkill(int courseId, [FromBody] ContentSkillTargetDto dto)
        {
            var result = await courseService.AddCourseSkillAsync(courseId, dto);
            return CreatedAtAction(nameof(GetById), new { id = courseId }, result);
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
        public async Task<IActionResult> AddTargetLearner(int courseId, [FromBody] ContentSkillTargetDto dto)
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