using AbySalto.Junior.DTOs;
using AbySalto.Junior.Exceptions;
using AbySalto.Junior.Infrastructure.Database;
using AbySalto.Junior.Repositories;
using AbySalto.Junior.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AbySalto.Junior.Tests.Integration
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private ApplicationDbContext _context;
        private OrderService _service;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
                .Options;

            _context = new ApplicationDbContext(options);
            var repository = new OrderRepository(_context);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var logger = new LoggerFactory().CreateLogger<OrderService>();
            _service = new OrderService(repository, cache, logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateOrder_ValidRequest_ReturnsOrderDto()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerName = "Test Korisnik",
                PaymentMethod = "Cash",
                DeliveryAddress = "Testna ulica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<CreateOrderItemDto>
                {
                    new() { Name = "Pizza", Quantity = 2, Price = 8.50m }
                }
            };

            // Act
            var result = await _service.CreateOrderAsync(dto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CustomerName, Is.EqualTo("Test Korisnik"));
            Assert.That(result.TotalAmount, Is.EqualTo(17.00m));
        }

        [Test]
        public async Task GetAllOrders_ReturnsAllOrders()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerName = "Test Korisnik",
                PaymentMethod = "Cash",
                DeliveryAddress = "Testna ulica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<CreateOrderItemDto>
                {
                    new() { Name = "Pizza", Quantity = 1, Price = 8.50m }
                }
            };

            // Act
            await _service.CreateOrderAsync(dto);
            var orders = await _service.GetAllOrdersAsync();

            // Assert
            Assert.That(orders.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetOrderById_NonExisting_ThrowsOrderNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsAsync<OrderNotFoundException>(async () =>
                await _service.GetOrderByIdAsync(999));
        }

        [Test]
        public async Task GetAllOrders_SortedByTotal_ReturnsSortedOrders()
        {
            // Arrange
            var dto1 = new CreateOrderDto
            {
                CustomerName = "Korisnik 1",
                PaymentMethod = "Cash",
                DeliveryAddress = "Ulica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<CreateOrderItemDto>
                {
                    new() { Name = "Pizza", Quantity = 1, Price = 5.00m }
                }
            };

            var dto2 = new CreateOrderDto
            {
                CustomerName = "Korisnik 2",
                PaymentMethod = "Card",
                DeliveryAddress = "Ulica 2, Zagreb",
                ContactNumber = "0987654321",
                Currency = "EUR",
                Items = new List<CreateOrderItemDto>
                {
                    new() { Name = "Steak", Quantity = 1, Price = 20.00m }
                }
            };

            // Act
            await _service.CreateOrderAsync(dto1);
            await _service.CreateOrderAsync(dto2);
            var orders = (await _service.GetAllOrdersAsync(sortByTotal: true)).ToList();

            // Assert
            Assert.That(orders.Count, Is.EqualTo(2));
            Assert.That(orders[0].TotalAmount, Is.GreaterThan(orders[1].TotalAmount));
        }

        [Test]
        public async Task UpdateOrderStatus_ValidStatus_ReturnsUpdatedOrder()
        {
            // Arrange
            var dto = new CreateOrderDto
            {
                CustomerName = "Test Korisnik",
                PaymentMethod = "Cash",
                DeliveryAddress = "Testna ulica 1, Zagreb",
                ContactNumber = "0911234567",
                Currency = "EUR",
                Items = new List<CreateOrderItemDto>
                {
                    new() { Name = "Pizza", Quantity = 1, Price = 8.50m }
                }
            };

            var created = await _service.CreateOrderAsync(dto);
            var updateDto = new UpdateOrderStatusDto { Status = Models.OrderStatus.InPreparation };

            // Act
            var result = await _service.UpdateOrderStatusAsync(created.Id, updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("InPreparation"));
        }
    }
}