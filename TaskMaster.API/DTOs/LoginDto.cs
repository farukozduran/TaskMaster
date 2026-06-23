using System.ComponentModel.DataAnnotations;

namespace TaskMaster.API.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Kullanici adi zorunludur.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Sifre zorunludur.")]
        public string Password { get; set; }
    }
}
