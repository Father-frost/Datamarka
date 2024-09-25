using Datamarka_DomainModel.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Datamarka_MVC.DataTransferObjects.Identity
{
    public class UserBriefDTO
    {
        [MaxLength(100)]
        [Required(ErrorMessage = "Не заполнено поле Логин!")]
        public required string UserName { get; set; }
        
        public required UserRoleEnum Role { get; set; }

        [MinLength(3, ErrorMessage = "Пароль слишком короткий!")]
        [MaxLength(50)]
        public required string Password { get; set; }

		[MinLength(3, ErrorMessage = "Пароль слишком короткий!")]
		[MaxLength(50)]
		public required string ConfirmPassword { get; set; }
	}
}
