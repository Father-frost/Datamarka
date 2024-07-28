using Datamarka_DAL.Repository;
using Datamarka_DomainModel.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Datamarka_DAL
{
    public interface IUnitOfWork
    {
        IDbContextTransaction BeginTransaction();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;

        Task<int> SaveChangesAsync();
    }
}
