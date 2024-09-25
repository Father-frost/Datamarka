using System.ComponentModel.DataAnnotations;

namespace Datamarka_MVC.DataTransferObjects.Identity
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Не указан логин")]
        public required string UserName { get; set;}

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
