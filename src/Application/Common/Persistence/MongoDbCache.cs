using Application.Common.Domain.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Application.Common.Persistence
{
    public class MongoDbCache : IMongoDbCache
    {
        private readonly IMongoCollection<BsonDocument> _cacheCollection;

        public MongoDbCache(IMongoClient client, IOptions<MongoOptions> mongoOptions)
        {
            var database = client.GetDatabase(mongoOptions.Value.Database);
            _cacheCollection = database.GetCollection<BsonDocument>(mongoOptions.Value.Collection);
        }

        public byte[] Get(string key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            var result = _cacheCollection.Find(filter).FirstOrDefault();
            return result == null ? null : result["Data"].AsByteArray;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            var result = await _cacheCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result == null ? null : result["Data"].AsByteArray;
        }

        public void Refresh(string key)
        {
            var newExpiration = DateTime.UtcNow.AddMinutes(20);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            var update = Builders<BsonDocument>.Update.Set("Expiration", newExpiration);
            _cacheCollection.UpdateOne(filter, update);
        }

        public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        {
            var newExpiration = DateTime.UtcNow.AddMinutes(20);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            var update = Builders<BsonDocument>.Update.Set("Expiration", newExpiration);
            await _cacheCollection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }

        public void Remove(string key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            _cacheCollection.DeleteOne(filter);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
            await _cacheCollection.DeleteOneAsync(filter, cancellationToken);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var doc = new BsonDocument
            {
                { "_id", key },
                { "Data", value },
                { "Expiration", DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(20)) }
            };
            _cacheCollection.ReplaceOne(Builders<BsonDocument>.Filter.Eq("_id", key), doc, new ReplaceOptions { IsUpsert = true });
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            var doc = new BsonDocument
            {
                { "_id", key },
                { "Data", value },
                { "Expiration", DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(20)) }
            };
            await _cacheCollection.ReplaceOneAsync(Builders<BsonDocument>.Filter.Eq("_id", key), doc, new ReplaceOptions { IsUpsert = true }, cancellationToken);
        }
    }
}
