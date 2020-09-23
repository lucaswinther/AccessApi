using System.ComponentModel.DataAnnotations;

namespace Access.Auth.Service.Domain.Request
{
    public class OldPortalNicknameChangeRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string NewNickname { get; set; }
    }
}
