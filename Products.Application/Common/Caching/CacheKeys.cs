namespace Products.Application.Common.Caching;

public static class CacheKeys
{
        public static string ProductById(Guid id)
        {
            return $"product:{id}";
        }
}