using System.ComponentModel.DataAnnotations;

namespace PersonalWebsite.Api.DTOs
{
    public class CreateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        public int BillToAddressId { get; set; }
        public int ShipToAddressId { get; set; }
        public int ShipMethodId { get; set; }
        [Required]
        [StringLength(1)]
        public string Notes { get; set; } = string.Empty;
    }
}
