using Microsoft.AspNetCore.Mvc;
using System;

namespace Access.Auth.Service.Host.Controllers
{
	[ApiController]
    [Route("")]
    public class MonitorController : ControllerBase
    {
        [HttpGet]
        [Route("[action]")]
        public IActionResult Health()
        {
            return Ok("v0.1");
        }
    }
}
