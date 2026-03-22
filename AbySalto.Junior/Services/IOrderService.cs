using AbySalto.Junior.DTOs;

namespace AbySalto.Junior.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(bool sortByTotal = false, CancellationToken ct = default);
        Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken ct = default);
        Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken ct = default);
    }
}
