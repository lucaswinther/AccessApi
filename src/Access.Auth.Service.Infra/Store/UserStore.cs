using System;
using System.Threading.Tasks;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Infra.Authentication;

namespace Access.Auth.Service.Infra.Store
{
    public class UserStore : IUserStore
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public UserStore(IAuthenticationRepository authenticationRepository) => this.authenticationRepository = authenticationRepository;

        public Task<User> FindUserByUsernameAsync(string username)
        {
            var user = this.authenticationRepository.Single<User>(u => u.Username == username.ToLower() && string.IsNullOrEmpty(u.Username) == false);
            return Task.FromResult(user);
        }

        public Task<User> FindUserByNicknameAsync(string nickname)
        {
            var user = this.authenticationRepository.Single<User>(u => u.Nickname == nickname.ToLower() && string.IsNullOrEmpty(u.Nickname) == false);
            return Task.FromResult(user);
        }

        public Task<User> FindUserByUsernameOrNicknameAsync(string name)
        {
            var user = this.authenticationRepository.Single<User>(u => (u.Username == name.ToLower() || u.Nickname == name.ToLower()));

            return Task.FromResult(user);
        }

        public Task<User> FindUserByIdAsync(string id)
        {
            var user = this.authenticationRepository.Single<User>(u => u.Id == id);
            return Task.FromResult(user);
        }

        public async Task AddAsync(User user) => await authenticationRepository.AddAsync(user);

        public async Task UpdateAsync(User user) => await authenticationRepository.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
