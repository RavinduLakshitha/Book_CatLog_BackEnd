namespace BookCatalog.Application.DTOs
{
    public class CreateBookDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Author { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}