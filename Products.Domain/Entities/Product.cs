using Products.Domain.Common;

namespace Products.Domain.Entities
{
    public sealed class Product : IAuditable
    {
        public Guid Id { get; private set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? UpdatedAtUtc { get; set; }
        public DateTimeOffset? DeletedAtUtc { get; private set; }

        private Product()
        {
        }

        public Product(string name, string description, decimal price, int stockQuantity)
        {
            Id = Guid.NewGuid();

            SetDetails(name, description);

            SetPrice(price);

            SetStockQuantity(stockQuantity);
        }

        public void SetDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void SetPrice(decimal price)
        {
            if (price < 0)
            {
                throw new ArgumentException("Price cannot be negative");
            }

            Price = price;
        }

        public void SetStockQuantity(int stockQuantity)
        {
            if (stockQuantity < 0)
            {
                throw new ArgumentException("Stock quantity cannot be negative");
            }

            StockQuantity = stockQuantity;
        }

        public void Delete()
        {
            if (IsDeleted)
            {
                return;
            }

            IsDeleted = true;

            DeletedAtUtc = DateTimeOffset.UtcNow;
        }
    }
}