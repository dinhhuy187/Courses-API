using courses_buynsell_api.DTOs.Cart;
using courses_buynsell_api.DTOs.Course;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace courses_buynsell_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin, Buyer")] 
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var cart = await cartService.GetCartAsync(userId);
            return Ok(cart);
        }
        
        [HttpPost("items/{courseId:int}")]
        [Authorize(Roles = "Admin, Buyer")]
        public async Task<IActionResult> AddItem(int courseId)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var cart = await cartService.AddItemAsync(userId, courseId);
            return Ok(cart);
        }
        
        [HttpDelete("items/{itemId:int}")]
        [Authorize(Roles = "Admin, Buyer")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var ok = await cartService.RemoveItemAsync(userId, itemId);
            if (!ok) return NotFound();
            return NoContent();
        }
        
        [HttpDelete]
        [Authorize(Roles = "Admin, Buyer")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var ok = await cartService.ClearCartAsync(userId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
