using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities;

namespace Talabat.BLL.ProductSpecifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        //Get All Product
        public ProductsWithTypesAndBrandsSpecification(ProductsSpecParams productsSpec)
            :base(
                 p => 
                 (string.IsNullOrEmpty(productsSpec.Search) || p.Name.ToLower().Contains(productsSpec.Search))
                 && (!productsSpec.BrandId.HasValue || p.ProductBrandId == productsSpec.BrandId.Value)
                 && (!productsSpec.TypeId.HasValue || p.ProductTypeId == productsSpec.TypeId.Value)
                 
                 )

        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
            AddOrderBy(p => p.Name);

            ApplyPaging(productsSpec.PageSize * (productsSpec.PageIndex - 1), productsSpec.PageSize);

            if (!string.IsNullOrEmpty(productsSpec.Sort))
            {
                switch (productsSpec.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;

                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }

        }

        //Get Specific Product
        public ProductsWithTypesAndBrandsSpecification(int id): base(p => p.Id == id)

        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);

        }
    }
}
