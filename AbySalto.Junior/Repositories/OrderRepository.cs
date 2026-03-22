using AbySalto.Junior.Infrastructure.Database;
using AbySalto.Junior.Models;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IApplicationDbContext _context;

        public OrderRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(ct);
            return order;
        }

        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ToListAsync(ct);
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);
        }

        public async Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(ct);
        }
    }
}
