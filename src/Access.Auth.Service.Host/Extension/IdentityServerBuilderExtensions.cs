using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Access.Auth.Service.Infra.Authentication;
using Access.Auth.Service.Infra.Store;
using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Access.Auth.Service.Domain.Configuration;
using Access.Auth.Service.Business.Validation;
using Access.Auth.Service.Business.UserManagement;
using Access.Auth.Service.Infra.Repositoires;
using Access.Auth.Service.Business.ClientManagement;
using Microsoft.AspNetCore.Authorization;
using Access.Auth.Service.Domain.Request;

namespace Access.Auth.Service.Host.Extension
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddTransient<IMongoConnection, MongoConnection>();
            return builder;
        }

        public static IIdentityServerBuilder AddClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, ClientStore>();
            builder.Services.AddTransient<IClientRepository, ClientRepository>();
            builder.Services.AddTransient<IClientManagementHandler, ClientManagementHandler>();
            builder.Services.AddTransient<ICorsPolicyService, InMemoryCorsPolicyService>();
            return builder;
        }

        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IResourceStore, ResourceStore>();
            return builder;
        }

        public static IIdentityServerBuilder AddUsers(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IUserManagementHandler, UserManagementHandler>();
            return builder;
        }

        public static IIdentityServerBuilder AddProfileService(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IProfileService, ProfileService>();
            return builder;
        }

        public static IIdentityServerBuilder AddResourceOwnerValidation(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            builder.Services.AddTransient<IUserStore, UserStore>();
            return builder;
        }

        public static IIdentityServerBuilder AddClientCredentialsValidation(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<ICustomTokenRequestValidator, ClientCredentialsRequestValidator>();
            return builder;
        }

        public static IIdentityServerBuilder AddExternalValidation(this IIdentityServerBuilder builder, IAuthConfiguration configuration)
        {
            builder.Services.AddTransient<IExternalValidationHandler, ExternalValidationHandler>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = configuration.IdentityServerBaseAddress;
                o.RequireHttpsMetadata = false;

                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudiences = new List<string>() {
                        configuration.ExternalValidationScope,
                        configuration.UserManagementScope
                    }
                };

                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var accessToken = context.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                var emailClaim = accessToken.Claims.FirstOrDefault(e => e.Type == JwtClaimTypes.Email);
                                if (emailClaim != null)
                                {
                                    identity.AddClaim(emailClaim);
                                }

                                var subjectClaim = accessToken.Claims.FirstOrDefault(e => e.Type == JwtClaimTypes.Subject);
                                if (subjectClaim != null)
                                {
                                    identity.AddClaim(subjectClaim);
                                }
                            }
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        o.ConfigurationManager.RequestRefresh();
                        return Task.CompletedTask;
                    },
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
                options.AddPolicy("RequireExternalValidationToken", policy => policy.RequireClaim("client_id", configuration.ExternalValidationClientId));
                options.AddPolicy("RequireUserManagementToken", policy => policy.RequireClaim("aud", configuration.UserManagementScope));
                options.AddPolicy("RequireUserManagementOrUserToken", policy => policy.RequireAssertion(
                    async context => await UserUpdatePolicyAsync(context, configuration)
                ));
            });

            return builder;
        }

        private static bool UserManagementOrAnalystValidation(AuthorizationHandlerContext context, IAuthConfiguration configuration)
        {
            if (context.User.HasClaim(c => c.Type == "aud" && c.Value == configuration.UserManagementScope)) { return true; }

            if (context.User.IsInRole("Analyst")) { return true; }

            return false;
        }

        private static async Task<bool> UserUpdatePolicyAsync(this AuthorizationHandlerContext context, IAuthConfiguration configuration)
        {
            if (UserManagementOrAnalystValidation(context, configuration)) { return true; }

            var claimUsername = context.User.Claims.Where(e => e.Type == "username").FirstOrDefault()?.Value;

            var requestUsername = (await context.GetResourceBodyAs<UserUpdateRequest>())?.Username;

            if (string.IsNullOrWhiteSpace(claimUsername) || string.IsNullOrWhiteSpace(requestUsername)) { return false; }

            if (requestUsername == claimUsername) { return true; }

            return false;
        }
    }
}
