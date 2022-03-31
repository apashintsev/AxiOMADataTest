using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repos
{
    public interface IRepository
    {
        Task AddResult(MongoAxiomaData data);
        Task<List<MongoAxiomaData>> GetAllResults();
    }

    public class Repository : IRepository
    {
        private readonly IMongoCollection<MongoAxiomaData> _data;

        public Repository(IMongoDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _data = database.GetCollection<MongoAxiomaData>(settings.DataCollectionName);
        }
        public async Task AddResult(MongoAxiomaData data)
        {
            await _data.InsertOneAsync(data);
        }

        public async Task<List<MongoAxiomaData>> GetAllResults()
        {
            return await _data.Find(x => true).ToListAsync();
        }
    }
}
