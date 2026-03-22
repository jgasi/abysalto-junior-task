using AbySalto.Junior.DTOs;
using AbySalto.Junior.Exceptions;
using AbySalto.Junior.Models;
using AbySalto.Junior.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace AbySalto.Junior.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<OrderService> _logger;

        private const string AllOrdersCacheKey = "all_orders";
        private const string AllOrdersSortedCacheKey = "all_orders_sorted";

        public OrderService(IOrderRepository orderRepository, IMemoryCache cache, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new order for customer: {CustomerName}", dto.CustomerName);

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                PaymentMethod = dto.PaymentMethod,
                DeliveryAddress = dto.DeliveryAddress,
                ContactNumber = dto.ContactNumber,
                Note = dto.Note,
                Currency = dto.Currency,
                OrderTime = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = dto.Items.Select(i => new OrderItem
                {
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            var created = await _orderRepository.CreateAsync(order, ct);

            _cache.Remove(AllOrdersCacheKey);
            _cache.Remove(AllOrdersSortedCacheKey);
            _logger.LogInformation("Order created successfully with ID: {OrderId}", created.Id);

            return MapToDto(created);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(bool sortByTotal = false, CancellationToken ct = default)
        {
            var cacheKey = sortByTotal ? AllOrdersSortedCacheKey : AllOrdersCacheKey;

            if (_cache.TryGetValue(cacheKey, out IEnumerable<OrderDto>? cachedOrders) && cachedOrders != null)
            {
                _logger.LogInformation("Returning orders from cache");
                return cachedOrders;
            }

            _logger.LogInformation("Cache miss - fetching orders from database");
            var orders = await _orderRepository.GetAllAsync(sortByTotal, ct);
            var dtos = orders.Select(MapToDto).ToList();

            _cache.Set(cacheKey, dtos, TimeSpan.FromMinutes(5));

            return dtos;
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken ct = default)
        {
            var cacheKey = $"order_{id}";

            if (_cache.TryGetValue(cacheKey, out OrderDto? cachedOrder) && cachedOrder != null)
            {
                _logger.LogInformation("Returning order {OrderId} from cache", id);
                return cachedOrder;
            }

            _logger.LogInformation("Cache miss - fetching order {OrderId} from database", id);
            var order = await _orderRepository.GetByIdAsync(id, ct);

            if (order is null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                throw new OrderNotFoundException(id);
            }

            var dto = MapToDto(order);
            _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));

            return dto;
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating status for order {OrderId} to {Status}", id, dto.Status);

            var order = await _orderRepository.GetByIdAsync(id, ct);

            if (order is null)
            {
                _logger.LogWarning("Order with ID {OrderId} not found", id);
                throw new OrderNotFoundException(id);
            }

            if (order.Status == OrderStatus.Completed)
            {
                throw new InvalidOrderStatusException($"Order {id} is already completed and cannot be updated.");
            }

            order.Status = (OrderStatus)dto.Status;
            await _orderRepository.UpdateAsync(order, ct);

            _cache.Remove(AllOrdersCacheKey);
            _cache.Remove(AllOrdersSortedCacheKey);
            _cache.Remove($"order_{id}");

            _logger.LogInformation("Order {OrderId} status updated successfully", id);

            return MapToDto(order);
        }

        private static OrderDto MapToDto(Order order) => new()
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Status = order.Status.ToString(),
            OrderTime = order.OrderTime,
            PaymentMethod = order.PaymentMethod,
            DeliveryAddress = order.DeliveryAddress,
            ContactNumber = order.ContactNumber,
            Note = order.Note,
            Currency = order.Currency,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }
}
