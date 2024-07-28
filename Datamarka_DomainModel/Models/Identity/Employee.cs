using Datamarka_DomainModel.Models.ECommerce;
using System.ComponentModel.DataAnnotations;

namespace Datamarka_DomainModel.Models.Identity
{
	public class Employee : Entity<long>
	{
		public required User User { get; set; }
		public required PostEnum Post { get; set; }
		public string? Skills { get; set; }

		public int? Experience {  get; set; }
		public string? Rating { get; set; }
	}
}
