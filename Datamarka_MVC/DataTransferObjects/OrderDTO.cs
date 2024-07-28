using Datamarka_DomainModel.Models.ECommerce;
using System.ComponentModel.DataAnnotations;

namespace Datamarka_BLL.Contracts
{
    public class OrderDTO
    {
		[MaxLength(100)]
		[MinLength(2, ErrorMessage = "The Order text is too short!")]
		[Required(ErrorMessage = "Order Text is required!")]
		public required long ProductId { get; set;}
		[Required(ErrorMessage = "Address is required!")]
		public DateTime ProdDate { get; set; }
		[Required(ErrorMessage = "Phone is required!")]
		public DateTime WarrantDate { get; set; }
    }
}
