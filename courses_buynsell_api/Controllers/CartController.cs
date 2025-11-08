using courses_buynsell_api.DTOs.Cart;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace courses_buynsell_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> Get(int userId)
        {
            var cart = await cartService.GetCartAsync(userId);
            if (cart == null) return Ok(new CartDto() { UserId = userId, Items = new List<CartItemDto>() }); // return empty to frontend
            return Ok(cart);
        }
        
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            var cart = await cartService.AddItemAsync(dto);
            return Ok(cart);
        }
        
        [HttpDelete("{userId:int}/items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int userId, int itemId)
        {
            var ok = await cartService.RemoveItemAsync(userId, itemId);
            if (!ok) return NotFound();
            return NoContent();
        }
        
        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var ok = await cartService.ClearCartAsync(userId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
