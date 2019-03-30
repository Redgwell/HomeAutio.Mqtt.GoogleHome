using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeAutio.Mqtt.GoogleHome.Identity.MongoStore
{
    /// <summary>
    /// Mongo db model for storing persisted grants.
    /// </summary>
    public class PersistedGrantDbModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [BsonElement("Key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [BsonElement("Type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>The subject identifier.</value>
        [BsonElement("SubjectId")]
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        [BsonElement("ClientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        [BsonElement("CreationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        [BsonElement("Expiration")]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [BsonElement("Data")]
        public string Data { get; set; }
    }
}