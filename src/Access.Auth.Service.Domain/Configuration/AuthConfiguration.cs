using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Access.Auth.Service.Domain.Configuration
{
    public class AuthConfiguration : IAuthConfiguration
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        private readonly string identityServerUri;
        
        public AuthConfiguration(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.configuration = configuration;

            connectionString = configuration["MongoConnectionString"];
            identityServerUri = configuration["IdentityServerUri"];
        }

        public string IdentityServerBaseAddress { get => identityServerUri; }
        public string ExternalValidationClientId { get => this.configuration.GetSection("ExternalValidation")["ClientId"]; }
        public string ExternalValidationSecret { get => this.configuration.GetSection("ExternalValidation")["ClientSecret"]; }
        public string ExternalValidationScope { get => this.configuration.GetSection("ExternalValidation")["ValidationScope"]; }
        public string UserManagementClientId { get => this.configuration.GetSection("UserManagement")["UserManagementClientId"]; }
        public string UserManagementScope { get => this.configuration.GetSection("UserManagement")["UserManagementScope"]; }
        public string DatabaseName { get => this.configuration["DatabaseName"]; }
        public string DatabaseConnectionString { get => connectionString; }
    }
}
