using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_DomainModel.Models.ECommerce
{
	public class Order : Entity<long>
	{
		public required string OrderCode { get; set; }
		public required User User { get; set; }
		//public string? UserId { get; set; }
		public required Product Product { get; set; }
		public decimal CodesCount { get; set; }
		public required DateTime ProdDate { get; set; }
		public required DateTime WarrantDate { get; set; }
		public required OrderStatusEnum Status { get; set; }
	}
}
