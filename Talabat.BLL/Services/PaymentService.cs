using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.BLL.OrderSpecifications;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Order;
using Product = Talabat.DAL.Entities.Product;

namespace Talabat.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IConfiguration _configuration;

        public PaymentService(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _configuration = configuration;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null)
                return null;

            var ShippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);

                ShippingPrice = deliveryMethod.Cost;
            }

            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                if (item.Price != productItem.Price)
                    item.Price = productItem.Price;
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100) + ((long)ShippingPrice + 100)),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100) + ((long)ShippingPrice + 100)),
                };

                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            basket.ShippingPrice = ShippingPrice;
            await _basketRepository.UpdateBasketAsync(basket);

            return basket;

        }

        public async Task<OrderEntity> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecification(paymentIntentId);

            var order = await _unitOfWork.Repository<OrderEntity>().GetEntityWithSpec(spec);

            if (order is null)
                return null;

            order.Status = OrderStatus.PaymentFailed;
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.Complete();

            return order;
        }

        public async Task<OrderEntity> UpdateOrderPaymentSucceded(string paymentIntentId)
        {
            var spec = new OrderWithItemsByPaymentIntentSpecification(paymentIntentId);

            var order = await _unitOfWork.Repository<OrderEntity>().GetEntityWithSpec(spec);

            if (order is null)
                return null;

            order.Status = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<OrderEntity>().Update(order);
            await _unitOfWork.Complete();

            return order;
        }
    }
}
