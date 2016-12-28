using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Interfaces.Services;
using TemplateMultiTenant.Domain.Models;
using System;
using System.Linq;

namespace TemplateMultiTenant.Domain.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : EntityBase
    {
        protected readonly IBaseRepository<TEntity> _entityRepository;
        protected readonly IUnityOfWork _unitOfWork;

        public BaseService(IBaseRepository<TEntity> entityRepository, IUnityOfWork unitOfWork)
        {
            _entityRepository = entityRepository;
            _unitOfWork = unitOfWork;
        }


        // Métodos

        public IQueryable<TEntity> GetAll()
        {
            return _entityRepository.GetAll();
        }

        public TEntity GetById(int id)
        {
            ServiceHelper.ValidateParams(new object[] { id });

            return _entityRepository.GetById(id);
        }

        public void Post(TEntity obj)
        {
            ServiceHelper.ValidateParams(new object[] { obj });

            //Verificando se existe
            if (GetById(obj.Id) != null)
            {
                throw new ArgumentException("object already exists");
            }
            
            _unitOfWork.BeginTrans();
            _entityRepository.Insert(obj);
            _unitOfWork.Commit();
        }

        public void Update(TEntity obj)
        {
            ServiceHelper.ValidateParams(new object[] { obj });
                        
            //Verificando se existe
            if (GetById(obj.Id) == null)
            {
                throw new ArgumentException("object not exists");
            }
                        
            _unitOfWork.BeginTrans();
            _entityRepository.Update(obj);
            _unitOfWork.Commit();
        }
    }
}
