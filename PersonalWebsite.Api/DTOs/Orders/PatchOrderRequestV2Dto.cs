namespace PersonalWebsite.Api.DTOs.Orders
{
    public class PatchOrderRequestV2Dto
    {
        public int? Status { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}
