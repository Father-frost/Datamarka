namespace Datamarka_DomainModel.Models.ECommerce
{
    public class CodesByOrder : Entity<long>
    {
        public required Product Order { get; set; }
        public required string Code { get; set; }
    }
}
