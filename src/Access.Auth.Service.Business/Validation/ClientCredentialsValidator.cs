using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Access.Auth.Service.Domain.Configuration;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Infra.Store;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Access.Auth.Service.Business.Validation
{
    public class ClientCredentialsRequestValidator : ICustomTokenRequestValidator
    {
        private readonly IAuthConfiguration authConfiguration;

        public ClientCredentialsRequestValidator(IAuthConfiguration authConfiguration) => this.authConfiguration = authConfiguration;

        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            var request = context.Result.ValidatedRequest;
            try
            {
                if (request.Client.ClientId == this.authConfiguration.ExternalValidationClientId)
                {
                    var email = request.Raw?["extra_email"];
                    if (string.IsNullOrEmpty(email) == false) { request.ClientClaims.Add(new Claim("email", email)); }
                    else
                    {
                        context.Result = new TokenRequestValidationResult(null, "invalid_email");
                    }
                }
            }
            catch
            {
                context.Result = new TokenRequestValidationResult(null, "client_validation_error");
            }

            return Task.CompletedTask;
        }
    }
}
