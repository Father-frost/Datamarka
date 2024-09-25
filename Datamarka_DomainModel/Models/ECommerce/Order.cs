using Datamarka_DomainModel.Models.Identity;

namespace Datamarka_DomainModel.Models.ECommerce
{
	public class Order : Entity<long>
	{
		//public required User User { get; set; }
		public required long UserId { get; set; }
		public required long ProductId { get; set; }
		public Product? Product { get; set; }
		public required string Batch { get; set; }
		public decimal CodesCount { get; set; }
		public required DateTime ProdDate { get; set; }
		public required DateTime WarrantDate { get; set; }
		public required OrderStatusEnum Status { get; set; }
	}
}
