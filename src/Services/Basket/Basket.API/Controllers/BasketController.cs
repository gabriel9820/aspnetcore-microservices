using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService)
        {
            _repository = repository;
            _discountGrpcService = discountGrpcService;
        }

        [HttpGet("{userName}")]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {
            // consume Discount.gRPC
            foreach (var item in shoppingCart.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            var updatedBasket = await _repository.UpdateBasket(shoppingCart);

            return Ok(updatedBasket);
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);

            return NoContent();
        }
    }
}