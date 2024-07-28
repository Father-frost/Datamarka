using Datamarka_DomainModel.Models.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Datamarka_MVC.DataTransferObjects.Identity
{
    public class UserBriefDTO
    {
        [MaxLength(100)]
        [MinLength(2, ErrorMessage = "The First Name is too short!")]
        [Required(ErrorMessage = "First Name is required!")]
        public required string FirstName { get; set; }

        [MaxLength(100)]
        [MinLength(2, ErrorMessage = "The Last Name is too short!")]
        public string? LastName { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Email is required!")]
        public required string Email { get; set; }
        
        public required UserRoleEnum Role { get; set; }

        [MinLength(10, ErrorMessage = "The password is too short!")]
        [MaxLength(50)]
        public required string Password { get; set; }
    }
}
