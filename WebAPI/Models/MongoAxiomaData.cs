using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class MongoAxiomaData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<AxiomaData> Data { get; set; }
        public BsonDateTime DateTime { get; set; }

    }
}
