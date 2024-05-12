using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class BasketItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string PictureUrl { get; set; }
        [Required]
        [Range(0.1, double.MaxValue,ErrorMessage ="Price Must be Greater Than 0 !!") ]

        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        [Range(0.1, int.MaxValue, ErrorMessage = "Quantity Must be Greater Than 0 !!")]
        public int Quantity { get; set; }
    }
}