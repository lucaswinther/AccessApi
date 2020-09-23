using System.ComponentModel.DataAnnotations;

namespace Access.Auth.Service.Domain.Request
{
    public class EmailChangeRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
