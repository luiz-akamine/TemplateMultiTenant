using System.Linq;
using TemplateMultiTenant.Domain.Models;

namespace TemplateMultiTenant.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    //public interface IBaseRepository
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(int id);
        void Insert(TEntity obj);
        void Update(TEntity obj);
        void Delete(TEntity obj);
        void Delete(int id);
    }
}
