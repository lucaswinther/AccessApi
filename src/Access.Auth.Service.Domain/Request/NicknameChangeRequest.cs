using System.ComponentModel.DataAnnotations;

namespace Access.Auth.Service.Domain.Request
{
    public class NicknameChangeRequest
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string NewNickname { get; set; }
    }
}
