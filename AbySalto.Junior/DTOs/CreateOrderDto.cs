namespace AbySalto.Junior.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string Currency { get; set; } = "EUR";
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
