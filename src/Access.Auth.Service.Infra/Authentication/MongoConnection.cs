using MongoDB.Driver;

using Access.Auth.Service.Domain.Configuration;
using System;
using Access.Auth.Service.Domain.Error;

namespace Access.Auth.Service.Infra.Authentication
{
    public class MongoConnection : IMongoConnection
    {
        private readonly IMongoDatabase mongoDatabase;

        public MongoConnection(IAuthConfiguration configuration)
        {
            try
            {
                var connectionString = configuration.DatabaseConnectionString;
                var client = new MongoClient(connectionString);
                this.mongoDatabase = client.GetDatabase(configuration.DatabaseName);
            }
            catch (Exception e)
            {
                throw new DatabaseException(e.Message);
            }
        }

        public IMongoCollection<T> OpenCollectionConnection<T>() where T : new()
        {
            try
            {
                var collectionName = typeof(T).Name;
                return this.mongoDatabase.GetCollection<T>(collectionName);
            }
            catch(Exception e)
            {
                throw new DatabaseException(e.Message);
            }
        }
    }
}
