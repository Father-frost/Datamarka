using Datamarka_DomainModel.Models.ECommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Datamarka_DAL.DbEntityConfigurations.ECommerce
{
    internal class CodesByOrderDBConfiguration : IEntityTypeConfiguration<CodesByOrder>
    {
        public void Configure(EntityTypeBuilder<CodesByOrder> builder)
        {

        }
    }
}
