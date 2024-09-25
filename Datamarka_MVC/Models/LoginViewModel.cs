using System.ComponentModel.DataAnnotations;

namespace Datamarka_MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? ReturnPath { get; set; }
    }
}
