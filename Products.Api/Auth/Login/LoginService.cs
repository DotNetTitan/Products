using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Products.Api.Auth.Interfaces;
using Products.Application.Abstractions;
using Products.Domain.Entities;

namespace Products.Api.Auth.Login;

public sealed class LoginService : ILoginService
{
    private readonly IApplicationDbContext _dbContext;

    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginService(IApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;

        _passwordHasher = passwordHasher;

        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult != PasswordVerificationResult.Success)
        {
            return null;
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResponse(token);
    }
}