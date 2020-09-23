using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Access.Auth.Service.Infra.Repositoires
{
    public interface IClientRepository
    {
        Task AddAsync(Client client);
    }
}