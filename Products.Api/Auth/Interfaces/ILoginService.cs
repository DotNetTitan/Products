using Products.Api.Auth.Login;

namespace Products.Api.Auth.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    }
}