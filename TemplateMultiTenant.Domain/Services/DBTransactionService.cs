using Microsoft.Practices.ServiceLocation;
using TemplateMultiTenant.Domain.Interfaces.Infra;

namespace TemplateMultiTenant.Domain.Services
{
    public class DBTransactionService
    {
        private IUnityOfWork _unityOfWork;

        public virtual void BeginTrans()
        {
            _unityOfWork = ServiceLocator.Current.GetInstance<IUnityOfWork>();
            _unityOfWork.BeginTrans();
        }

        public virtual void Commit()
        {
            _unityOfWork.Commit();
        }
    }
}