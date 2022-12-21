using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.OrderSpecifications;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order;

namespace Talabat.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<OrderEntity> CreateOrderAsync(string buyerEmail, 
            int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // 1. Get Basket from Basket Repo
            var basket = await _basketRepository.GetBasketAsync(basketId);
            // 2. Get Selected Items at Basket from Product Repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                
                var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                items.Add(orderItem);
            }

            // 3. Get Delivery Method from DeliveryMey=thod Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 4. Calculate SubTotal
            var subTotal = items.Sum(item => item.Price * item.Quantity);

            // Check to see if order is exists
            var spec = new OrderWithItemsByPaymentIntentSpecification(basket.PaymentIntentId);
            var existOrder = await _unitOfWork.Repository<OrderEntity>().GetEntityWithSpec(spec);

            if (existOrder != null)
            {
                _unitOfWork.Repository<OrderEntity>().Delete(existOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);

            }

            // 5. Create Order
            var order = new OrderEntity(buyerEmail, shippingAddress, deliveryMethod, items, subTotal, basket.PaymentIntentId);
            _unitOfWork.Repository<OrderEntity>().Add(order);

            // 6. Save to Database
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return null;

            return order;

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
            => await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        public async Task<OrderEntity> GetOrderByIdAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecification(orderId, buyerEmail);
            return await _unitOfWork.Repository<OrderEntity>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<OrderEntity>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecification(buyerEmail);
            return await _unitOfWork.Repository<OrderEntity>().GetAllWithSpecAsync(spec);
        }
    }
}
