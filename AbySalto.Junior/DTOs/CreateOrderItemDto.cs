namespace AbySalto.Junior.DTOs
{
    public class CreateOrderItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
