//Classe POCO contendo propriedades necessárias para autenticação do client
using TemplateMultiTenant.Auth.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace TemplateMultiTenant.Auth.ViewModel
{
    public class ClientModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]                
        public long CNPJ { get; set; }

        [Required]
        public SubscriptionType SubscriptionType { get; set; }        
    }


}