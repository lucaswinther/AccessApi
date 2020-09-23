using Access.Auth.Service.Domain.Error;
using System;
using System.Net;

namespace Access.Auth.Service.Domain.UserManagement
{
    public class UserManagementException : Exception
    {
        public UserManagementException(string message) : base(message)
        {
        }
    }
}
