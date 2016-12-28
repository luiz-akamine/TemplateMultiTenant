using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace TemplateMultiTenant.Domain.Models
{
    public class Product : EntityBase    
    {
        //public int Id { get; set; }

        //[Column(TypeName = "char(10)")]
        [Required]
        public string Code { get; set; }

        //[Column(TypeName = "char(100)")]
        [Required]
        public string Name { get; set; }

        public double Price { get; set; }

        public int ProductType { get; set; }

        public int test { get; set; }
    }
}