using AbySalto.Junior.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.CustomerName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(o => o.Status)
                   .HasConversion<int>();

            builder.Property(o => o.OrderTime)
                   .IsRequired();

            builder.Property(o => o.PaymentMethod)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(o => o.DeliveryAddress)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(o => o.ContactNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(o => o.Note)
                   .HasMaxLength(1000);

            builder.Property(o => o.Currency)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Ignore(o => o.TotalAmount);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
