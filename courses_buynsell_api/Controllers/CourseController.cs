using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace courses_buynsell_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CourseQueryParameters queryParameters)
        {
            var result = await _courseService.GetCoursesAsync(queryParameters);
            return Ok(result);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
        {
            var created = await _courseService.CreateAsync(createCourseDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            var updated = await _courseService.UpdateAsync(id, updateCourseDto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _courseService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}