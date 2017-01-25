using System.Data.Entity;
using System.Linq;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Models;
using TemplateMultiTenant.Infra.Configuration;

namespace TemplateMultiTenant.Infra.Repositories
{
    public class BaseRepository<TEntity>: IBaseRepository<TEntity> where TEntity : EntityBase
    {
        public BaseRepository() { }


        // Métodos

        public IQueryable<TEntity> GetAll()
        {            
            return RepositoryManager.Context.Set<TEntity>();
        }

        public TEntity GetById(int id)
        {
            return RepositoryManager.Context.Set<TEntity>().Where(x => x.Id == id).FirstOrDefault();
        }

        public void Insert(TEntity obj)
        {
            RepositoryManager.Context.Set<TEntity>().Add(obj);
        }

        public void Update(TEntity obj)
        {                       
            var entity = RepositoryManager.Context.Set<TEntity>().Find(obj.Id);
            if (entity == null)
            {
                return;
            }

            RepositoryManager.Context.Entry<TEntity>(entity).CurrentValues.SetValues(obj);
            RepositoryManager.Context.Entry<TEntity>(entity).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            TEntity obj = GetById(id);
            if (obj != null)
            {
                Delete(obj);
            }
        }

        public void Delete(TEntity obj)
        {
            RepositoryManager.Context.Set<TEntity>().Remove(obj);
        }
    }    
}