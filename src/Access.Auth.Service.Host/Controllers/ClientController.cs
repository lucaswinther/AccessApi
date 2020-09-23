using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Access.Auth.Service.Domain.Request;
using Access.Auth.Service.Business.ClientManagement;
using System.Net;
using System.Linq;

namespace Access.Auth.Service.Host.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ClientController : ControllerBase
  {
    private readonly IClientManagementHandler clientManagementHandler;

    public ClientController(IClientManagementHandler clientManagementHandler) =>
      this.clientManagementHandler = clientManagementHandler;

    [HttpPost]
    [Route("[action]")]
    [Authorize(Policy = "RequireUserManagementToken")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<ActionResult> AddOmniClient([FromBody] AddClientRequest request)
    {
      var client = await this.clientManagementHandler.AddClientAsync(request);

      return this.Created("client", client);
    }
  }
}