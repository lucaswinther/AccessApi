using System.ComponentModel.DataAnnotations;

namespace Access.Auth.Service.Domain.Request
{
    public class UserUpdateRequest
    {
        [Required]
        public string Username { get; set; }
    }
}
