using MongoDB.Driver;

namespace Access.Auth.Service.Infra.Authentication
{
    public interface IMongoConnection
    {
        IMongoCollection<T> OpenCollectionConnection<T>() where T : new();
    }
}