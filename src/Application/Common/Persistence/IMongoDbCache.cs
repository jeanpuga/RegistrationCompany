using Microsoft.Extensions.Caching.Distributed;

namespace Application.Common.Persistence
{
    public interface IMongoDbCache
    {
        byte[] Get(string key);
        Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default);
        void Refresh(string key);
        Task RefreshAsync(string key, CancellationToken cancellationToken = default);
        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        void Set(string key, byte[] value, DistributedCacheEntryOptions options);
        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);
    }
}