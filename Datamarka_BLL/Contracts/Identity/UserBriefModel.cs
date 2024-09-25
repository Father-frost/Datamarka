using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_BLL.Contracts.Identity
{
	public class UserBriefModel
	{
		public long? Id { get; set; }
		public required string UserName { get; set; }
		public required UserRoleEnum Role { get; set; }
	}
}
