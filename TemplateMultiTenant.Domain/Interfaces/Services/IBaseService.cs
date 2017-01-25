using System.Linq;

namespace TemplateMultiTenant.Domain.Interfaces.Services
{
    public interface IBaseService<TEntity>
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(int id);
        void Post(TEntity obj);
        void Update(TEntity obj);
        void Delete(TEntity obj);
        void Delete(int id);
    }
}
