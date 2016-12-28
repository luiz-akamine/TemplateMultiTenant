using System;

namespace TemplateMultiTenant.Domain.Services
{
    public static class ServiceHelper
    {
        public static void ValidateParams(object[] objs)
        {
            foreach (object obj in objs)
            {
                if (obj == null)
                {
                    throw new ArgumentException("Error: invalid/null parameter");
                }
            }
        }
    }
}
