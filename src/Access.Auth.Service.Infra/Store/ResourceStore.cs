using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Access.Auth.Service.Infra.Authentication;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Access.Auth.Service.Infra.Store
{
    public class ResourceStore : IResourceStore
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public ResourceStore(IAuthenticationRepository authenticationRepository)
            => this.authenticationRepository = authenticationRepository;

        private IEnumerable<ApiResource> GetAllApiResources()
            => this.authenticationRepository.All<ApiResource>();

        private IEnumerable<IdentityResource> GetAllIdentityResources()
            => this.authenticationRepository.All<IdentityResource>();

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return string.IsNullOrEmpty(name)
                ? throw new ArgumentNullException(nameof(name))
                : Task.FromResult(this.authenticationRepository.Single<ApiResource>(a => a.Name == name));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var apiResources = this.authenticationRepository.Where<ApiResource>(a => a.Scopes.Any(s => scopeNames.Contains(s.Name)));
            return Task.FromResult(apiResources.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var identityResources = this.authenticationRepository.Where<IdentityResource>(e => scopeNames.Contains(e.Name));
            return Task.FromResult(identityResources.AsEnumerable());
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var resources = new Resources(this.GetAllIdentityResources(), this.GetAllApiResources());
            return Task.FromResult(resources);
        }

        private Func<IdentityResource, bool> BuildPredicate(Func<IdentityResource, bool> predicate) => predicate;
    }
}
