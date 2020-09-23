using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Infra.Store;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Access.Auth.Service.Business.Validation
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserStore userStore;

        public ResourceOwnerPasswordValidator(IUserStore userStore)
        {
            this.userStore = userStore;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await this.userStore.FindUserByUsernameOrNicknameAsync(context.UserName);

                if (user == null || string.IsNullOrWhiteSpace(context.UserName))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_user_credentials");
                    return;
                }
            
                if (!BCrypt.Net.BCrypt.Verify(context.Password, user.Password))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_user_credentials");
                    return;
                }

                context.Result = new GrantValidationResult(
                    subject: user.Id,
                    authenticationMethod: "custom",
                    claims: GetUserClaims(user)
                );
            }
            catch
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "user_validation_error");
            }
        }

        public static Claim[] GetUserClaims(User user)
        {
            var claims = new List<Claim>() {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim("username", !string.IsNullOrEmpty(user.Username) ? user.Username : ""),
                new Claim(JwtClaimTypes.Name, !string.IsNullOrEmpty(user.Name) ? user.Name : ""),
                new Claim(JwtClaimTypes.Email, !string.IsNullOrEmpty(user.Email) ? user.Email : ""),
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            if (user.Claims == null) { return claims.ToArray(); }
            
            foreach (var claim in user.Claims)
            {
                foreach (var value in claim.Value as IEnumerable)
                {
                    claims.Add(new Claim(claim.Key, value as string));
                }
            }

            return claims.ToArray();
        }
    }
}
