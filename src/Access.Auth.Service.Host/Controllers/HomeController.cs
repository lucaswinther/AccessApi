using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Access.Auth.Service.Host.Models;
using IdentityModel.Client;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace Access.Auth.Service.Host.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService interaction;

        public HomeController(IIdentityServerInteractionService interaction) => this.interaction = interaction;

        public IActionResult Index() => View();

        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
