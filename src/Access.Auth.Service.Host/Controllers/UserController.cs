using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Access.Auth.Service.Host.Models;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

using Access.Auth.Service.Domain.Configuration;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Business.UserManagement;
using Access.Auth.Service.Domain.Request;

namespace Access.Auth.Service.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManagementHandler userManagementHandler;

        public UserController(IUserManagementHandler userManagementHandler) => this.userManagementHandler = userManagementHandler;

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> Add(User user)
        {
            await userManagementHandler.AddAsync(user);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUser")]
        public async Task<ActionResult> ChangePassword(PasswordChangeRequest request)
        {
            var subject = User.Claims.FirstOrDefault(e => e.Type == JwtClaimTypes.Subject);
            await userManagementHandler.ChangePasswordAsync(request, subject.Value);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUser")]
        public async Task<ActionResult> ChangeNickname(NicknameChangeRequest request)
        {
            var subject = User.Claims.FirstOrDefault(e => e.Type == JwtClaimTypes.Subject);
            await userManagementHandler.ChangeNicknameAsync(request, subject.Value);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementOrUserToken")]
        public async Task<ActionResult> ChangeName(NameChangeRequest request)
        {
            await userManagementHandler.ChangeNameAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementOrUserToken")]
        public async Task<ActionResult> ChangeEmail(EmailChangeRequest request)
        {
            await userManagementHandler.ChangeEmailAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> AddCnpj(CnpjUpdateRequest request)
        {
            await userManagementHandler.AddCnpjAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> RemoveCnpj(CnpjUpdateRequest request)
        {
            await userManagementHandler.RemoveCnpjAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> AddProduct(ProductUpdateRequest request)
        {
            await userManagementHandler.AddProductAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> RemoveProduct(ProductUpdateRequest request)
        {
            await userManagementHandler.RemoveProductAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> AddRole(RoleUpdateRequest request)
        {
            await userManagementHandler.AddRoleAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> RemoveRole(RoleUpdateRequest request)
        {
            await userManagementHandler.RemoveRoleAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("changePassword_old")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> OldPortalChangePassword(OldPortalPasswordChangeRequest request)
        {
            await userManagementHandler.OldPortalChangePasswordAsync(request);
            return Ok();
        }

        [HttpPost]
        [Route("changeNickname_old")]
        [Authorize(Policy = "RequireUserManagementToken")]
        public async Task<ActionResult> OldPortalChangeNickname(OldPortalNicknameChangeRequest request)
        {
            await userManagementHandler.OldPortalChangeNicknameAsync(request);
            return Ok();
        }
    }
}
