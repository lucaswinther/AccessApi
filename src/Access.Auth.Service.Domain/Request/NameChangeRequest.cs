using System.ComponentModel.DataAnnotations;

namespace Access.Auth.Service.Domain.Request
{
    public class NameChangeRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
