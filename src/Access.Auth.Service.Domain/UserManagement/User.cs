using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Access.Auth.Service.Domain.UserManagement
{
    public class User
    {
        private const string EMAIL_REGEX = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
        
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        [RegularExpression(EMAIL_REGEX, ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        
        private string username;
        [Required]
        public string Username { get => username; set => username = value.ToLower(); }
        
        private string nickname;
        public string Nickname { get => nickname; set => nickname = value?.ToLower(); }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public List<string> Roles { get; set; }
   
        public Dictionary<string, IEnumerable<string>> Claims { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(this.Id)) { throw new UserManagementException(UserErrorMessage.INVALID_ID); }
            if (this.Nickname != null && (this.Nickname.Contains(" ") || string.IsNullOrEmpty(this.Nickname))) { throw new UserManagementException(UserErrorMessage.INVALID_NICKNAME); }
            if (this.Username.Contains(" ") || string.IsNullOrEmpty(this.Username)) { throw new UserManagementException(UserErrorMessage.INVALID_USERNAME); }
            if (this.Password == null) { throw new UserManagementException(UserErrorMessage.SHORT_PASSWORD); }
            if (this.Password.Length < 6) { throw new UserManagementException(UserErrorMessage.SHORT_PASSWORD); }
           
            var regex = new Regex(EMAIL_REGEX);
            if (regex.Match(this.Email).Success == false) { throw new UserManagementException(UserErrorMessage.INVALID_EMAIL); }

            ValidateRoles();
        }

        private void ValidateRoles()
        {
            if (this.Roles == null || this.Roles.Any() == false) { throw new UserManagementException(UserErrorMessage.INVALID_ROLE); }
            
            var properties = typeof(UserManagement.Roles).GetProperties();
            foreach (var role in Roles)
            {
                if (!properties.Any(e => e.Name == role)) { throw new UserManagementException(UserErrorMessage.INVALID_ROLE); }
            }
        }
    }
}
