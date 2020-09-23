using System.ComponentModel.DataAnnotations;

namespace Access.Auth.Service.Domain.Request
{
    public class OldPortalPasswordChangeRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
