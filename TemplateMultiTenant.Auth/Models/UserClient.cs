using System.ComponentModel.DataAnnotations;

namespace TemplateMultiTenant.Auth.Models
{
    public class UserClient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }        
        [Required]        
        public Client Client { get; set; }
    }
}