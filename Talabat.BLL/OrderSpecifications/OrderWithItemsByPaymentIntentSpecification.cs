using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities.Order;

namespace Talabat.BLL.OrderSpecifications
{
    public class OrderWithItemsByPaymentIntentSpecification : BaseSpecification<OrderEntity>
    {
        public OrderWithItemsByPaymentIntentSpecification(string paymentIntentId) 
            : base(o => o.PaymentIntentId == paymentIntentId)
        {

        }
    }
}
