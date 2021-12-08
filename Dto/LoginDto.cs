using System.ComponentModel.DataAnnotations;

namespace Electronic_Organizer_API.Dto
{
    public class LoginDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
