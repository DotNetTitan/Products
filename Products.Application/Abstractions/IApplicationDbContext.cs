using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;

namespace Products.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken);
    }
}