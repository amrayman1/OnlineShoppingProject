using System.ComponentModel.DataAnnotations;
using Talabat.DAL.Entities;

namespace Talabat.API.Dtos
{
    public class BasketItemDto 
    {
        [Required]
        public int Id { get; set; }
        public string ProductName { get; set; }
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Price must greater than zero")]
        public decimal Price { get; set; }
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Quantity must greater than zero")]
        public int Quantity { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        [Required]
        public string Brand { get; set; }

    }
}