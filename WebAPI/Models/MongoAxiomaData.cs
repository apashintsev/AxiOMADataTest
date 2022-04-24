using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class MongoAxiomaData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public string Id { get; set; }
        public List<AxiomaData> Data { get; set; }
        public DateTime DateTime { get; set; }

    }
}
