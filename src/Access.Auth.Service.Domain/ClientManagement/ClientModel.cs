using System;
using System.Collections.Generic;

namespace Access.Auth.Service.Domain
{
    public class ClientModel
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> AllowedScopes { get; set; }
        public List<string> GrantTypes { get; set; }
    }
}