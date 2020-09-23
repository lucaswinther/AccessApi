using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Access.Auth.Service.Host
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations()
        {
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                this.Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            this.SigningCredentials = new SigningCredentials(this.Key, SecurityAlgorithms.RsaSha256Signature);
        }
    }
}
