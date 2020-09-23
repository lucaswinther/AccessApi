using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Access.Auth.Service.Domain;
using Access.Auth.Service.Domain.Request;
using Access.Auth.Service.Infra.Repositoires;
using Access.Auth.Service.Business.UserManagement;

namespace Access.Auth.Service.Business.ClientManagement
{
	public class ClientManagementHandler : IClientManagementHandler
	{
		private readonly IClientRepository clientRepository;

		public ClientManagementHandler(IClientRepository clientRepository)
		  => this.clientRepository = clientRepository;

		public async Task<ClientModel> AddClientAsync(AddClientRequest request)
		{
			var client = new Client
			{
				ClientName = request.ClientName,
				ClientId = request.ClientId,
				ClientSecrets = new List<Secret> { new Secret(request.ClientSecret.Sha256()) },
				AllowedScopes = request.AllowedScopes,
				AllowedGrantTypes = request.GrantTypes
			};

			await this.clientRepository.AddAsync(client);

			return new ClientModel
			{
				ClientName = request.ClientName,
				ClientId = request.ClientId,
				ClientSecret = request.ClientSecret,
				AllowedScopes = request.AllowedScopes.ToList(),
				GrantTypes = request.GrantTypes.ToList()
			};
		}
	}
}