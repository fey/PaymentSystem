using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Login not defined")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Password not defined")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}