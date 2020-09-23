using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Access.Auth.Service.Domain.UserManagement;

namespace Access.Auth.Service.Domain.Request
{
    public class ProductUpdateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public List<string> Products { get; set; }

        public void ValidateProducts()
        {
            if (Products == null || !Products.Any()) { throw new UserManagementException(UserErrorMessage.INVALID_PRODUCT); }
        }
    }
}
