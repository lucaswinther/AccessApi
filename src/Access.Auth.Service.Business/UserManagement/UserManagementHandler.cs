using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Infra.Store;
using BCrypt.Net;
using System.Linq;

using Access.Auth.Service.Domain.Request;
using Access.Auth.Service.Business.Extension;

namespace Access.Auth.Service.Business.UserManagement
{
    public class UserManagementHandler : IUserManagementHandler
    {
        private readonly IUserStore userStore;

        public UserManagementHandler(IUserStore userStore) => this.userStore = userStore;

        public async Task AddAsync(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            user.Validate();

            if (await GetUserByUsernameOrNickname(user) != null) { throw new UserManagementException(UserErrorMessage.EXISTING_USER); }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await userStore.AddAsync(user);
        }

        public async Task ChangePasswordAsync(PasswordChangeRequest request, string userId)
        {
            if (string.IsNullOrEmpty(userId)) { throw new UserManagementException(UserErrorMessage.USER_ID_NOT_FOUND); }
            if (request.NewPassword != request.NewPasswordConfirmation) { throw new UserManagementException(UserErrorMessage.PASSWORD_CONFIRMATION_FAILED); }

            var user = await userStore.FindUserByIdAsync(userId) ?? throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND);
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password)) { throw new UserManagementException(UserErrorMessage.INVALID_CURRENT_PASSWORD); }

            user.Password = request.NewPassword;
            user.Validate();
            
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await userStore.UpdateAsync(user);
        }

        public async Task ChangeNicknameAsync(NicknameChangeRequest request, string userId)
        {
            if (string.IsNullOrEmpty(userId)) { throw new UserManagementException(UserErrorMessage.USER_ID_NOT_FOUND); }
            if (await userStore.FindUserByUsernameOrNicknameAsync(request.NewNickname) != null) { throw new UserManagementException(UserErrorMessage.INVALID_NICKNAME); }

            var user = await userStore.FindUserByIdAsync(userId) ?? throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND);
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) { throw new UserManagementException(UserErrorMessage.INVALID_CURRENT_PASSWORD); }

            user.Nickname = request.NewNickname;
            user.Validate();

            await userStore.UpdateAsync(user);
        }

        public async Task ChangeNameAsync(NameChangeRequest request)
        {
            var user = await userStore.FindUserByUsernameAsync(request.Username) ?? throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND);
            user.Name = request.Name;
            user.Validate();

            await userStore.UpdateAsync(user);
        }

        public async Task ChangeEmailAsync(EmailChangeRequest request)
        {
            var user = await userStore.FindUserByUsernameAsync(request.Username) ?? throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND);
            user.Email = request.Email;
            user.Validate();

            await userStore.UpdateAsync(user);
        }

        public async Task AddCnpjAsync(CnpjUpdateRequest request)
        {
            request.ValidateCnpjs();
            const string key = "cnpjs";
            await AddClaimsAsync(request.Username, request.Cnpjs, e => e.Claims.GetOrDefault(key), (u, e) => u.Claims[key] = e);
        }

        public async Task RemoveCnpjAsync(CnpjUpdateRequest request)
        {
            request.ValidateCnpjs();
            const string key = "cnpjs";
            await RemoveClaimsAsync(request.Username, request.Cnpjs, e => e.Claims.GetOrDefault(key), (u, e) => u.Claims[key] = e);
        }

        public async Task AddProductAsync(ProductUpdateRequest request)
        {
            request.ValidateProducts();
            const string key = "products";
            await AddClaimsAsync(request.Username, request.Products, e => e.Claims.GetOrDefault(key), (u, e) => u.Claims[key] = e);
        }

        public async Task RemoveProductAsync(ProductUpdateRequest request)
        {
            request.ValidateProducts();
            const string key = "products";
            await RemoveClaimsAsync(request.Username, request.Products, e => e.Claims.GetOrDefault(key), (u, e) => u.Claims[key] = e);
        }

        public async Task AddRoleAsync(RoleUpdateRequest request)
        {
            request.ValidateRoles();
            await AddClaimsAsync(request.Username, request.Roles, e => e.Roles, (u, e) => u.Roles = e.ToList());
        }

        public async Task RemoveRoleAsync(RoleUpdateRequest request)
        {
            request.ValidateRoles();
            await RemoveClaimsAsync(request.Username, request.Roles, e => e.Roles, (u, e) => u.Roles = e.ToList());
        }

        private async Task AddClaimsAsync(string username, IEnumerable<string> claims, Func<User, IEnumerable<string>> extract, Action<User, IEnumerable<string>> update)
        {              
            var (user, claimsList) = await GetUserAndClaims(username, extract);

            claimsList.AddRange(claims);
            var distintic = claimsList.Distinct();
            update(user, distintic);

            await userStore.UpdateAsync(user);
        }

        private async Task RemoveClaimsAsync(string username, IEnumerable<string> claims, Func<User, IEnumerable<string>> extract, Action<User, IEnumerable<string>> update)
        {              
            var (user, claimsList) = await GetUserAndClaims(username, extract);

            claimsList.RemoveAll(e => claims.Contains(e));
            var distintic = claimsList.Distinct();
            update(user, distintic);
            
            await userStore.UpdateAsync(user);
        }

        private async Task<(User, List<string>)> GetUserAndClaims(string username, Func<User, IEnumerable<string>> extract)
        {
            var user = await userStore.FindUserByUsernameAsync(username);
            if (user == null) { throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND); }
            
            var claimsEnumerable = extract(user);
            if (claimsEnumerable == null) { claimsEnumerable = new List<string>(); }

            return (user, claimsEnumerable.ToList());
        }

        public async Task OldPortalChangePasswordAsync(OldPortalPasswordChangeRequest request)
        {
            var user = await userStore.FindUserByUsernameAsync(request.Username);
            if (user == null) { throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND); }
            user.Password = request.NewPassword;

            user.Validate();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await userStore.UpdateAsync(user);
        }

        public async Task OldPortalChangeNicknameAsync(OldPortalNicknameChangeRequest request)
        {
            var byNickname = await userStore.FindUserByNicknameAsync(request.NewNickname);
            if (byNickname != null) { throw new UserManagementException(UserErrorMessage.NICKNAME_UNAVAILABLE); }

            var user = await userStore.FindUserByUsernameAsync(request.Username);
            if (user == null) { throw new UserManagementException(UserErrorMessage.USER_NOT_FOUND); }

            user.Nickname = request.NewNickname;

            user.Validate();
            await userStore.UpdateAsync(user);
        }

        private async Task<User> GetUserByUsernameOrNickname(User user)
        {
            var byUsername = userStore.FindUserByUsernameOrNicknameAsync(user.Username);
            var byNickname = user.Nickname == null ? null : userStore.FindUserByUsernameOrNicknameAsync(user.Nickname);

            await byUsername;

            if (byNickname != null) { await byNickname; }

            return byUsername.Result ?? byNickname?.Result;
        }
    }
}
