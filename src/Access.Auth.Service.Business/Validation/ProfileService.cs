using System;
using System.Linq;
using System.Threading.Tasks;
using Access.Auth.Service.Infra.Store;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Access.Auth.Service.Business.Validation
{
    public class ProfileService : IProfileService
    {
        private readonly IUserStore userStore;

        public ProfileService(IUserStore userStore)
        {
            this.userStore = userStore;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims = context.Subject.Claims.ToList();
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
