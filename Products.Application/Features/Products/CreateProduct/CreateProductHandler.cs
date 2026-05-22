using Products.Application.Abstractions;
using Products.Domain.Entities;

namespace Products.Application.Features.Products.CreateProduct
{
    public sealed class CreateProductHandler(IApplicationDbContext dbContext)
        : ICommandHandler<CreateProductCommand, CreateProductResponse>
    {
        private readonly IApplicationDbContext _dbContext = dbContext;

        public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product(
                command.Name,
                command.Description,
                command.Price,
                command.StockQuantity);

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateProductResponse(product.Id);
        }
    }
}