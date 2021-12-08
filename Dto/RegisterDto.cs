using System.ComponentModel.DataAnnotations;

namespace Electronic_Organizer_API.Dto
{
    public class RegisterDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Avatar is required.")]
        public string Avatar { get; set; }
    }
}
