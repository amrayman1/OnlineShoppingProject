using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities;

namespace Talabat.BLL.ProductSpecifications
{
    public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFiltersForCountSpecification(ProductsSpecParams productsSpec)
            : base(
                 p =>
                 (string.IsNullOrEmpty(productsSpec.Search) || p.Name.ToLower().Contains(productsSpec.Search))
                 && (!productsSpec.BrandId.HasValue || p.ProductBrandId == productsSpec.BrandId.Value)
                 && (!productsSpec.TypeId.HasValue || p.ProductTypeId == productsSpec.TypeId.Value)

                 )
        {

        }
    }
}
