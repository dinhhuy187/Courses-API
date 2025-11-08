using courses_buynsell_api.DTOs.Cart;
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
            if (cart == null) return Ok(new CartDto() { UserId = userId, Items = new List<CartItemDto>() }); // return empty to frontend
            return Ok(cart);
        }
        
        [HttpPost("items")]
        [Authorize(Roles = "Admin, Buyer")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            if (dto.UserId != null)
            {
                if (userId != dto.UserId)
                    return BadRequest("UserId not match, something goes wrong");
            }
            else dto.UserId = userId;
            var cart = await cartService.AddItemAsync(dto);
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
