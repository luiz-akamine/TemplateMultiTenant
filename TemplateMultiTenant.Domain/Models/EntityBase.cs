using System;
using System.ComponentModel.DataAnnotations;

namespace TemplateMultiTenant.Domain.Models
{
    public class EntityBase : IDisposable
    {
        //[Key]
        public int Id { get; set; }

        public void Dispose()
        {
            
        }
    }
}
