using Newtonsoft.Json;
using System;
using TemplateMultiTenant.Domain.Models;

namespace TemplateMultiTenant.API.Controllers
{
    public static class ControllerHelper
    {
        // Método auxiliar para conversão dos parâmetros
        public static T ConvertRequestObject<T>(RequestBase request)
        {
            try
            { 
                if (request.Params != null)
                {
                    return JsonConvert.DeserializeObject<T>(request.Params.ToString());
                }

                throw new ArgumentNullException("Params is null in Request Object");
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }        
    }
}