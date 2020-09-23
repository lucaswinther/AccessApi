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
using Access.Auth.Service.Business.Validation;

namespace Access.Auth.Service.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly IExternalValidationHandler externalValidationHandler;

        public ExternalController(IExternalValidationHandler externalValidationHandler) => this.externalValidationHandler = externalValidationHandler;

        [HttpGet]
        [Authorize(Policy = "RequireExternalValidationToken")]
        [Route("[action]")]
        public ActionResult Validate() => Ok();

        [HttpGet]
        [Authorize(Policy = "RequireUser")]
        [Route("[action]")]
        public async Task<ActionResult> Token()
        {
            var emailClaim = User.Claims.FirstOrDefault(e => e.Type == JwtClaimTypes.Email);
            if (emailClaim == null) { return BadRequest(new { error = "invalid_email" }); }

            var token = await this.externalValidationHandler.GetExternalValidationToken(emailClaim.Value);
            return Ok(token);
        }
    }
}
