namespace TemplateMultiTenant.Domain.Interfaces.Infra
{
    public interface IUnityOfWork
    {
        void BeginTrans();
        void Commit();
    }
}
