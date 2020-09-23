using System;
using System.Collections.Generic;

namespace Access.Auth.Service.Domain.Request
{
    public class AddClientRequest
    {
        public AddClientRequest(string clientName, 
          ICollection<string> grantTypes, 
          ICollection<string> allowedScopes, 
          string clientId = null,
          string clientSecret = null)
        {
            ClientName = clientName ?? throw new ArgumentNullException(nameof(ClientName));
            GrantTypes = grantTypes ?? throw new ArgumentNullException(nameof(GrantTypes));
            AllowedScopes = allowedScopes ?? throw new ArgumentNullException(nameof(AllowedScopes));

            ClientId = clientId ?? Guid.NewGuid().ToString("N");            
            ClientSecret = clientSecret ?? Guid.NewGuid().ToString("N");
        }
        
        public string ClientName { get; }
        public ICollection<string> GrantTypes { get; }
        public ICollection<string> AllowedScopes { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
    }
}