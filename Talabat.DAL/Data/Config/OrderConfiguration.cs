using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Order;

namespace Talabat.DAL.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {

        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.OwnsOne(O => O.ShipToAddress, NP =>
            {
                NP.WithOwner();
            });
            
            builder.Property(O => O.Status)
                .HasConversion(
                    orderStatus => orderStatus.ToString(),
                    orderStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus)
                    );

            builder.HasMany(O => O.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
                
            
        }
    }
}
