using System.ComponentModel.DataAnnotations;

namespace auth_web_api.Models.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
