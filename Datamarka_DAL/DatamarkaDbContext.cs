using Datamarka_DomainModel.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace Datamarka_DAL
{
    public class DatamarkaDbContext : DbContext
    {
        public DatamarkaDbContext(DbContextOptions<DatamarkaDbContext> options) : base(options)
        {

		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
