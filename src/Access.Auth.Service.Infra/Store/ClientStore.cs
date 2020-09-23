using System;
using System.Linq;
using System.Threading.Tasks;
using Access.Auth.Service.Infra.Authentication;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Access.Auth.Service.Infra.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public ClientStore(IAuthenticationRepository authenticationRepository)
            => this.authenticationRepository = authenticationRepository;

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = this.authenticationRepository.Single<Client>(c => c.ClientId == clientId);
            return Task.FromResult(client);
        }
    }
}
