using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Data;
using Talabat.DAL.Entities.Order;

namespace Talabat.DAL.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
			try
			{
				if (!context.ProductBrands.Any())
				{
					var brandData = File.ReadAllText("../Talabat.DAL/Data/SeedData/brands.json");
					var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
					foreach (var brand in brands)
						await context.ProductBrands.AddAsync(brand);
					await context.SaveChangesAsync();
				}

                if (!context.ProductTypes.Any())
                {
                    var typeData = File.ReadAllText("../Talabat.DAL/Data/SeedData/types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                    foreach (var type in types)
                        await context.ProductTypes.AddAsync(type);
                    await context.SaveChangesAsync();
                }

                if (!context.Products.Any())
                {
                    var productData = File.ReadAllText("../Talabat.DAL/Data/SeedData/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productData);
                    foreach (var product in products)
                        await context.Products.AddAsync(product);
                    await context.SaveChangesAsync();
                }

                if (!context.DeliveryMethods.Any())
                {
                    var deliveryMethods = File.ReadAllText("../Talabat.DAL/Data/SeedData/delivery.json");
                    var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethods);
                    foreach (var method in methods)
                        await context.DeliveryMethods.AddAsync(method);
                    await context.SaveChangesAsync();
                }


            }
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<StoreContextSeed>();
				logger.LogError(ex.Message);
			}
        }
    }
}
