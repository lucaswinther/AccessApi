using System.Threading.Tasks;
using Access.Auth.Service.Domain.Configuration;
using IdentityModel.Client;
using IdentityServer4.Stores;
using Newtonsoft.Json.Linq;

namespace Access.Auth.Service.Business.Validation
{
    public class ExternalValidationHandler : IExternalValidationHandler
    {
        private readonly IClientStore clientStore;
        private readonly IAuthConfiguration configuration;

        public ExternalValidationHandler(IClientStore clientStore, IAuthConfiguration configuration)
        {
            this.clientStore = clientStore;
            this.configuration = configuration;
        }

        public async Task<JObject> GetExternalValidationToken(string email)
        {
            var client = await this.clientStore.FindClientByIdAsync(this.configuration.ExternalValidationClientId);
            var tokenClient = new TokenClient($"{this.configuration.IdentityServerBaseAddress}/connect/token", client.ClientId, this.configuration.ExternalValidationSecret);

            var response = await tokenClient.RequestClientCredentialsAsync(this.configuration.ExternalValidationScope, new { extra_email = email });
            return response.Json;
        }
    }
}
