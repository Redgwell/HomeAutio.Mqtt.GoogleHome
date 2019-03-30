using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HomeAutio.Mqtt.GoogleHome.Identity.MongoStore
{
    /// <summary>
    /// Persisted grant store backed by MongoDb.
    /// </summary>
    public class MongoPersistedGrantStore : IPersistedGrantStoreWithExpiration
    {
        private readonly ILogger<MongoPersistedGrantStore> _log;
        private readonly IMongoCollection<PersistedGrantDbModel> _grants;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPersistedGrantStore"/> class.
        /// </summary>
        /// <param name="logger">Logging instance.</param>
        /// <param name="configuration">Conffguration.</param>
        public MongoPersistedGrantStore(ILogger<MongoPersistedGrantStore> logger, IConfiguration configuration)
        {
            _log = logger ?? throw new ArgumentException(nameof(logger));
            if (configuration == null)
                throw new ArgumentException(nameof(configuration));

            _log.LogInformation("Setting up Mongo persisted grant store");

            var connectionString = configuration.GetValue<string>("oauth:tokenStore:connectionString");
            var databaseName = configuration.GetValue<string>("oauth:tokenStore:database");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _grants = database.GetCollection<PersistedGrantDbModel>("Tokens");
        }

        private PersistedGrant ToEntity(PersistedGrantDbModel model)
        {
            return new PersistedGrant
            {
                Key = model.Key,
                Type = model.Type,
                SubjectId = model.SubjectId,
                ClientId = model.ClientId,
                CreationTime = model.CreationTime,
                Expiration = model.Expiration,
                Data = model.Data
            };
        }

        private PersistedGrantDbModel ToModel(PersistedGrant entity, ObjectId? existingId)
        {
            return new PersistedGrantDbModel
            {
                Id = existingId ?? ObjectId.GenerateNewId(),
                Key = entity.Key,
                Type = entity.Type,
                SubjectId = entity.SubjectId,
                ClientId = entity.ClientId,
                CreationTime = entity.CreationTime,
                Expiration = entity.Expiration,
                Data = entity.Data
            };
        }

        /// <inheritdoc />
        public async Task StoreAsync(PersistedGrant grant)
        {
            var existingGrant = (await _grants.FindAsync(g => g.Key == grant.Key)).FirstOrDefault();

            await _grants.ReplaceOneAsync(
                g => g.Key == grant.Key,
                ToModel(grant, existingGrant?.Id),
                new UpdateOptions { IsUpsert = true });
        }

        /// <inheritdoc />
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var matches = await _grants.FindAsync(g => g.Key == key);
            var grant = matches.FirstOrDefault();
            if (grant != null)
            {
                return ToEntity(grant);
            }

            _log.LogWarning("Failed to find token with key {key}", key);
            return null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var matches = await _grants.FindAsync(g => g.SubjectId == subjectId);

            return matches.ToEnumerable().Select(ToEntity);
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string key)
        {
            await _grants.DeleteManyAsync(g => g.Key == key);
        }

        /// <inheritdoc />
        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await _grants.DeleteManyAsync(g => g.SubjectId == subjectId && g.ClientId == clientId);
        }

        /// <inheritdoc />
        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await _grants.DeleteManyAsync(g => g.SubjectId == subjectId && g.ClientId == clientId && g.Type == type);
        }

        /// <inheritdoc />
        public async Task RemoveAllExpiredAsync()
        {
            await _grants.DeleteManyAsync(g => g.Expiration < DateTime.UtcNow);
        }
    }
}
