using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Models;
using TemplateMultiTenant.Infra.Configuration;
using TemplateMultiTenant.Infra.Context;
//using Microsoft.Data.Entity;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace TemplateMultiTenant.Infra.Repositories
{
    public class BaseRepository<TEntity>: IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly TemplateMultiTenantContext _context;

        public BaseRepository()
        {
            // install-package commonservicelocator
            var repositoryManager = (RepositoryManager)ServiceLocator.Current.GetInstance<IRepositoryManager>();
            _context = repositoryManager.Context;
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
            _context.Set<TEntity>().Remove(obj);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Where(x => x.Id == id).FirstOrDefault();
        }

        public void Insert(TEntity obj)
        {
            _context.Set<TEntity>().Add(obj);
        }

        public void Update(TEntity obj)
        {
            /* AJUSTE Entity Framework 7
             //Gambiarra ou Ajuste Técnico??? Sem isso abaixo, não funciona... Ocorre o erro:
             //
             //  'Entidade XXX' cannot be tracked because another instance of this type with the same key 
             //  is already being tracked. For new entities consider using an IIdentityGenerator to generate 
             //  unique key values.
             //
             //Assim, estou "Detachando" a entidade em questão para ser salva
             var entry = _context.ChangeTracker.Entries().Where(x => x.Entity.GetType().Equals(obj.GetType())).FirstOrDefault();
             _context.Entry(entry.Entity).State = EntityState.Detached;

             _context.Entry(obj).State = EntityState.Modified;
            */

            //Entity Framework 6
            var entity = _context.Set<TEntity>().Find(obj.Id);
            if (entity == null)
            {
                return;
            }

            _context.Entry(entity).CurrentValues.SetValues(obj);
        }
    }    
}