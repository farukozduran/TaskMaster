using System.ComponentModel.DataAnnotations;

namespace TaskMaster.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Kullanici adi zorunludur.")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email zorunludur.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Sifre zorunludur.")]
        public string Password { get; set; }

        public string FullName { get; set; }
    }
}
