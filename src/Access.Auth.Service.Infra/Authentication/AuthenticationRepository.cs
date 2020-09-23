using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Access.Auth.Service.Infra.Authentication
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IMongoConnection mongoConnection;

        public AuthenticationRepository(IMongoConnection mongoConnection)
            => this.mongoConnection = mongoConnection ?? throw new ArgumentNullException(nameof(mongoConnection));

        public IQueryable<T> All<T>() where T : class, new() => this.mongoConnection.OpenCollectionConnection<T>().AsQueryable();

        public bool CollectionExists<T>() where T : class, new()
        {
            var collection = this.mongoConnection.OpenCollectionConnection<T>();
            var filter = new BsonDocument();
            var totalCount = collection.CountDocuments(filter);
            return totalCount > 0;
        }

        public T Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
            => this.All<T>().Where(expression).SingleOrDefault();

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
            => this.All<T>().Where(expression);

        public async Task AddAsync<T>(T document) where T : class, new()
        {
            var collection = this.mongoConnection.OpenCollectionConnection<T>();
            await collection.InsertOneAsync(document);
        }

        public async Task ReplaceOneAsync<T>(Expression<Func<T, bool>> expression, T document) where T : class, new()
        {
            var collection = this.mongoConnection.OpenCollectionConnection<T>();
            await collection.ReplaceOneAsync(expression, document);
        }
    }
}
