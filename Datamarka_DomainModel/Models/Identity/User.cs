using System.ComponentModel.DataAnnotations;

namespace Datamarka_DomainModel.Models.Identity
{
	public class User : IEntity
	{

		public long Id { get; set; } = default!;

		[MaxLength(100)]
		public required string? UserName { get; set; }

		public required UserSettings UserSettings { get; set; }

		public required UserRoleEnum Role { get; set; }
		public required string Password { get; set; }
	}
}
