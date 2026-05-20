namespace Products.Application.Common.Caching;

public static class CacheKeys
{
    public static string ProductById(Guid Id)
    {
        return $"product:{Id}";
    }
}