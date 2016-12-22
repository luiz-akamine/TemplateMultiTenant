using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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