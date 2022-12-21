using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Helper;
using Talabat.BLL.Interfaces;
using Talabat.BLL.ProductSpecifications;
using Talabat.BLL.Specifications;
using Talabat.DAL.Entities;

namespace Talabat.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productRepo, 
            IGenericRepository<ProductType> productTypeRepo, 
            IGenericRepository<ProductBrand> productBrandRepo,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _productTypeRepo = productTypeRepo;
            _productBrandRepo = productBrandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [CachedAttribute(600)]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery] ProductsSpecParams productsSpec)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productsSpec);

            var countSpec = new ProductsWithFiltersForCountSpecification(productsSpec);

            var totalItems = await _productRepo.GetCountAsync(countSpec);
            
            var products = await _productRepo.GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);

            return new Pagination<ProductDto>(productsSpec.PageIndex, productsSpec.PageSize, totalItems, data);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            
            var product = await _productRepo.GetEntityWithSpec(spec);

            if (product is null)
                return NotFound(new ApiExecption(404));

            var data = _mapper.Map<Product, ProductDto>(product);

            return Ok(data);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _productBrandRepo.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]
        [CachedAttribute(10)]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _productTypeRepo.GetAllAsync();
            return Ok(types);
        }

    }
}
