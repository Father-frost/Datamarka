using Datamarka_MVC.Enums;

namespace Datamarka_MVC.Models
{
	public class EmployeeViewModel
	{
		public long Id { get; set; }
		public PostEnum Post { get; set; }
		public string? Skills { get; set; }
        public int? Experience { get; set; }
        public string? Rating { get; set; }

    }
}
