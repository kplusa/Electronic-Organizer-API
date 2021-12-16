using System.ComponentModel.DataAnnotations;

namespace Electronic_Organizer_API.Dto
{
    public class ExternalLoginDto
    {
        [Required(ErrorMessage = "TokenId is required.")]
        public string TokenId { get; set; }
    }
}
