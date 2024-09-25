using Datamarka_DomainModel.Models.ECommerce;
using System.ComponentModel.DataAnnotations;

namespace Datamarka_BLL.Contracts
{
    public class OrderDTO
    {
		public required long ProductId { get; set;}
		[Required(ErrorMessage = "Заполните дату изготовления!")]
		public DateTime ProdDate { get; set; }
		[Required(ErrorMessage = "Заполните дату окончания срока годности!")]
		public DateTime WarrantDate { get; set; }
		[Required(ErrorMessage = "Заполните партию!")]
		public required string Batch { get; set; }
    }
}
