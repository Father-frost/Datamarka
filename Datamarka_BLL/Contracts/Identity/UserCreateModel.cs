namespace Datamarka_BLL.Contracts.Identity
{
	public class UserCreateModel : UserBriefModel
	{
		public required string Password { get; set; }
	}
}
