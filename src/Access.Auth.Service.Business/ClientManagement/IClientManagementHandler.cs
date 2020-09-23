using System;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Access.Auth.Service.Domain.Request;
using Access.Auth.Service.Domain;

namespace Access.Auth.Service.Business.ClientManagement
{
    public interface IClientManagementHandler
    {
        Task<ClientModel> AddClientAsync(AddClientRequest addOmniClientRequest);
    }  
}
