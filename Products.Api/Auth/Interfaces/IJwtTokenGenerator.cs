using Products.Domain.Entities;

namespace Products.Api.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
