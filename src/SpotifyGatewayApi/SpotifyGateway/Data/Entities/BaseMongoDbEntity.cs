using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SpotifyGateway.Data.Entities
{
    public class BaseMongoDbEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }

        public DateTime CreationTime = DateTime.UtcNow;

        public string MachineName = Environment.MachineName;
    }
}