using System.ComponentModel.DataAnnotations;

namespace PaymentSystem.Model.Dto.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Повтор пароля введен неверно")]
        public string PasswordConfirmation { get; set; }
    }
}