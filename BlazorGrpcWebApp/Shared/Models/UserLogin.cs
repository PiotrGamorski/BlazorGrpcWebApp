using System.ComponentModel.DataAnnotations;

namespace BlazorGrpcWebApp.Shared.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter an Email Address.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }    
    }
}
