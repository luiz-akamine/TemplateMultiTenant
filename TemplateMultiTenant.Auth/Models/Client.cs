//Classe para implementação do "Refresh Token"
//Esta classe nomeada como "Client", refere-se aos clientes que irão consumir as APIs do lado do servidor. Ex: Browsers, Celulares, Desktops, etc
using System;
using System.ComponentModel.DataAnnotations;

namespace TemplateMultiTenant.Auth.Models
{
    public class Client
    {
        [Key]
        [Required]
        public string Id { get; set; } //id client
        [Required]
        public string Secret { get; set; } //Hash do "Id" Client
        public ApplicationTypes ApplicationType { get; set; } //ver classe Enum
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } //Nome client       
        public bool Active { get; set; } //0 = client desativado não podendo mais requisitar APIs | 1 = client ativo
        public int RefreshTokenLifeTime { get; set; } //tempo de vida do token
        [MaxLength(100)]
        public string AllowedOrigin { get; set; } //configuração do CORs (de onde será aceito ser chamada as APIs)
        public string ConnectionString { get; set; }
        public SubscriptionType SubscriptionType { get; set; }        
        public int CNPJ { get; set; }
        public DateTime DtCreation { get; set; }
        public DateTime? DtLastUpdate { get; set; }
        public DateTime? DtSubscriptionExpiration { get; set; }        
    }

    public enum ApplicationTypes
    {
        //Websites por exemplo
        JavaScript = 0,
        //Aplicações denominadas "confidenciais", como por exemplo dispositivos móveis, celulares, tablets, computador pessoal, etc
        NativeConfidential = 1
    };

    //Tipo de assinatura do cliente
    public enum SubscriptionType
    {
        Free = 0,
        Basic = 1,
        Premium = 2
    }
}