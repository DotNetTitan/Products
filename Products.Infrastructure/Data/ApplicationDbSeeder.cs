using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Products.Domain.Entities;
using Products.Domain.Enums;

namespace Products.Infrastructure.Data;

public sealed class ApplicationDbSeeder
{
    private readonly ApplicationDbContext _dbContext;

    private readonly IPasswordHasher<User> _passwordHasher;

    private const string AdminEmail = "admin@products.com";
    private const string AdminPassword = "Admin123!";

    public ApplicationDbSeeder(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;

        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        var existingAdmin = await _dbContext.Users
            .AnyAsync(x => x.Email == AdminEmail);

        if (existingAdmin)
        {
            return;
        }

        var passwordHash = _passwordHasher
            .HashPassword(new User(AdminEmail, string.Empty, Role.Admin), AdminPassword);

        var adminUser = new User(
            AdminEmail,
            passwordHash,
            Role.Admin);

        _dbContext.Users.Add(adminUser);

        await _dbContext.SaveChangesAsync();
    }
}