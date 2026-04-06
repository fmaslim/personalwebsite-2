namespace PersonalWebsite.Api.DTOs
{
    /*
     * Monday, 04/06/2024 2:17 PM
     
     * Business case

You’re building an API for a store system.

The frontend needs a page that shows a list of products.

But in real life:

products table can get big
frontend does not need every column
results should be sorted
results should be limited

So we want a good first GET endpoint.

    Your task

Build this endpoint:
    GET /api/products

     */
    public class ProductListDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? ListPrice { get; set; }
    }
}
