namespace Products.Api.Auth.Login;

public sealed record LoginRequest(
    string Email,
    string Password);