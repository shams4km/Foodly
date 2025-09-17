using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Foodly.Infrastructure.Services
{
    public class MongoLogService
    {
        private readonly IMongoCollection<BsonDocument> _logs;

        public MongoLogService(IConfiguration cfg)
        {
            var conn = cfg.GetConnectionString("Mongo");
            var client = new MongoClient(conn);
            var db = client.GetDatabase("foodly_logs");
            _logs = db.GetCollection<BsonDocument>("request_logs");
        }

        public Task WriteAsync(string? userId, string path, string method, int statusCode, long elapsedMs)
        {
            var doc = new BsonDocument
            {
                { "userId", userId ?? "" },
                { "path", path },
                { "method", method },
                { "status", statusCode },
                { "elapsedMs", elapsedMs },
                { "ts", DateTime.UtcNow }
            };
            return _logs.InsertOneAsync(doc);
        }
    }
}
