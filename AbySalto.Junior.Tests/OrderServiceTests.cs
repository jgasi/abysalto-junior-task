using AbySalto.Junior.DTOs;
using AbySalto.Junior.Exceptions;
using AbySalto.Junior.Models;
using AbySalto.Junior.Repositories;
using AbySalto.Junior.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace AbySalto.Junior.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _repositoryMock;
        private Mock<IMemoryCache> _cacheMock;
        private Mock<ILogger<OrderService>> _loggerMock;
        private OrderService _orderService;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _cacheMock = new Mock<IMemoryCache>();
            _loggerMock = new Mock<ILogger<OrderService>>();


            object? cacheValue = null;
            _cacheMock
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
                .Returns(false);

            _cacheMock
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            _orderService = new OrderService(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateOrderAsync_ValidDto_ReturnsOrderDto()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerName = "Ivan Horvat",
                PaymentMethod = "Cash",
                DeliveryAddress = "Ilica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<CreateOrderItemDto>
                {
                    new() { Name = "Pizza", Quantity = 2, Price = 8.50m }
                }
            };

            _repositoryMock
                .Setup(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order o, CancellationToken _) => o);

            // Act
            var result = await _orderService.CreateOrderAsync(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CustomerName, Is.EqualTo("Ivan Horvat"));
            Assert.That(result.Status, Is.EqualTo("Pending"));
            Assert.That(result.TotalAmount, Is.EqualTo(17.00m));
        }

        [Test]
        public async Task GetOrderByIdAsync_ExistingId_ReturnsOrderDto()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerName = "Ivan Horvat",
                Status = OrderStatus.Pending,
                PaymentMethod = "Cash",
                DeliveryAddress = "Ilica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<OrderItem>
                {
                    new() { Id = 1, Name = "Pizza", Quantity = 1, Price = 8.50m }
                }
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.CustomerName, Is.EqualTo("Ivan Horvat"));
        }

        [Test]
        public void GetOrderByIdAsync_NonExistingId_ThrowsOrderNotFoundException()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act & Assert
            Assert.ThrowsAsync<OrderNotFoundException>(async () =>
                await _orderService.GetOrderByIdAsync(99));
        }

        [Test]
        public void UpdateOrderStatusAsync_CompletedOrder_ThrowsInvalidOrderStatusException()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerName = "Ivan Horvat",
                Status = OrderStatus.Completed,
                PaymentMethod = "Cash",
                DeliveryAddress = "Ilica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<OrderItem>()
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var dto = new UpdateOrderStatusDto { Status = (int)OrderStatus.Pending };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOrderStatusException>(async () =>
                await _orderService.UpdateOrderStatusAsync(1, dto));
        }

        [Test]
        public async Task UpdateOrderStatusAsync_ValidStatus_ReturnsUpdatedOrderDto()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                CustomerName = "Ivan Horvat",
                Status = OrderStatus.Pending,
                PaymentMethod = "Cash",
                DeliveryAddress = "Ilica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<OrderItem>()
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var dto = new UpdateOrderStatusDto { Status = (int)OrderStatus.InPreparation };

            // Act
            var result = await _orderService.UpdateOrderStatusAsync(1, dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("InPreparation"));
        }
    }
}