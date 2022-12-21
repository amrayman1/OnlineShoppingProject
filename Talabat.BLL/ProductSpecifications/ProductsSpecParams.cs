using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.BLL.Specifications
{
    public class ProductsSpecParams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? Sort { get; set; }
        public int PageIndex { get; set; } = 1;

        private const int MaxPageSize = 50;
        
        private int _pageSize = 5;
        public int PageSize
        {
            get { return _pageSize; }
            set 
            { 
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value.ToLower(); }
        }



    }
}
