using Products.Domain.Common;
using Products.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Products.Domain.Entities;

public sealed class User : IAuditable
{
    public Guid Id { get; private set; }

    public string Email { get; private set; } = default!;

    public string PasswordHash { get; private set; } = default!;

    public Role Role { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public bool IsDeleted { get; private set; }

    private User()
    {
    }

    public User(string email, string passwordHash, Role role)
    {
        Id = Guid.NewGuid();

        Email = email;

        PasswordHash = passwordHash;

        Role = role;
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