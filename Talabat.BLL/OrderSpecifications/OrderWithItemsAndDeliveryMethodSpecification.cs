using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities.Order;

namespace Talabat.BLL.OrderSpecifications
{
    public class OrderWithItemsAndDeliveryMethodSpecification : BaseSpecification<OrderEntity>
    {
        public OrderWithItemsAndDeliveryMethodSpecification(string email) : base(o => o.BuyerEmail == email)
        {
            AddInclude(o => o.Items);
            AddInclude(o => o.DeliveryMethod);
        }
        
        public OrderWithItemsAndDeliveryMethodSpecification(int id, string email) : base(o => o.Id == id && o.BuyerEmail == email)
        {
            AddInclude(o => o.Items);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}
