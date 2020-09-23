using System;
using System.Threading.Tasks;
using Access.Auth.Service.Infra.Authentication;
using IdentityServer4.Models;

namespace Access.Auth.Service.Infra.Repositoires 
{
  public class ClientRepository : IClientRepository 
  {
    private readonly IAuthenticationRepository authenticationRepository;

    public ClientRepository(IAuthenticationRepository authenticationRepository)
      => this.authenticationRepository = authenticationRepository;

    public async Task AddAsync (Client client) => await authenticationRepository.AddAsync (client);
  }
}