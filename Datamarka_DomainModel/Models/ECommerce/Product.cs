
namespace Datamarka_DomainModel.Models.ECommerce
{
    public class Product : Entity<long>
    {
        public required string GTIN { get; set; }
        public required string Name { get; set; }
        public required string KPR { get; set; }
        public required decimal BestBeforedays { get; set; }
        public required decimal CountInPack { get; set; }
        public required decimal CountInPallet { get; set; }

    }
}
