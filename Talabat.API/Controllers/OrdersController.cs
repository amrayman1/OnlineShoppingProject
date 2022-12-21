using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.BLL.Interfaces;
using Talabat.DAL.Entities.Order;

namespace Talabat.API.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost("PlaceOrder")]
        public async Task<ActionResult<OrderEntity>> PlaceOrder(OrderDto orderDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryMethodId, orderDto.BasketId, address);

            if (order is null)
                return BadRequest(new ApiResponse(400, "A problem Occured While Placing Order!"));

            return Ok(order);
        }

        [HttpGet("GetOrdersForUser")]
        public async Task<ActionResult<IReadOnlyList<UserOrderDto>>> GetOrdersForUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForUserAsync(email);
            return Ok(_mapper.Map<IReadOnlyList<OrderEntity>, IReadOnlyList<UserOrderDto>>(orders));
        }

        [HttpGet("GetOrderByIdForUser/id")]
        public async Task<ActionResult<UserOrderDto>> GetOrderByIdForUser(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdAsync(id, email);
            if(order is null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<OrderEntity, UserOrderDto>(order));
        }

        [HttpGet("GetDeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();

            return Ok(deliveryMethods);
        }


    }
}
