using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Access.Auth.Service.Domain.UserManagement;

namespace Access.Auth.Service.Domain.Request
{
    public class RoleUpdateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public List<string> Roles { get; set; }

        public void ValidateRoles()
        {
            if (Roles == null || !Roles.Any()) { throw new UserManagementException(UserErrorMessage.INVALID_ROLE); }
            
            var properties = typeof(UserManagement.Roles).GetProperties();
            foreach (var role in Roles)
            {
                if (!properties.Any(e => e.Name == role)) { throw new UserManagementException(UserErrorMessage.INVALID_ROLE); }
            }
        }
    }
}
