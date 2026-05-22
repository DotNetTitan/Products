using System.Collections.Concurrent;
using Products.Application.Abstractions;

namespace Products.UnitTests.Application.Handlers;

public sealed class TestCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object?> _cache = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult((T?)value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        _cache[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
