using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Enums;

namespace Products.Infrastructure.Data;

public sealed class ApplicationDbSeeder
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IPasswordHasher<User> _passwordHasher;

    public ApplicationDbSeeder(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;

        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        var existingAdmin = await _dbContext.Users
            .AnyAsync(x => x.Email == "admin@products.com");

        if (existingAdmin)
        {
            return;
        }

        var tempAdminUser = new User("admin@products.com", string.Empty, Role.Admin);

        var passwordHash = _passwordHasher.HashPassword(tempAdminUser, "Admin123!");

        var adminUser = new User(
            tempAdminUser.Email,
            passwordHash,
            tempAdminUser.Role);

        _dbContext.Users.Add(adminUser);

        await _dbContext.SaveChangesAsync();
    }
}