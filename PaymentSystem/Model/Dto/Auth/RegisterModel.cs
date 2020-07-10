using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Login not defined")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password not defined")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string PasswordConfirmation { get; set; }
    }
}