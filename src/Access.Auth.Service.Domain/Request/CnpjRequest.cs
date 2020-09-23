using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Access.Auth.Service.Domain.UserManagement;

namespace Access.Auth.Service.Domain.Request
{
    public class CnpjUpdateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public List<string> Cnpjs { get; set; }

        public void ValidateCnpjs()
        {
            if (Cnpjs == null || !Cnpjs.Any()) { throw new UserManagementException(UserErrorMessage.INVALID_CNPJ); }
            
            var regex = new Regex(@"^[0-9]{14}$");
            foreach (var cnpj in Cnpjs)
            {
                if (!regex.Match(cnpj).Success) { throw new UserManagementException(UserErrorMessage.INVALID_CNPJ); }
            }
        }
    }
}
