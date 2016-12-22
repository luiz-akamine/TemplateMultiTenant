using System;
using System.ComponentModel.DataAnnotations;

namespace TemplateMultiTenant.Auth.Models
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; } //refresh token id
        [Required]
        [MaxLength(50)]
        public string Subject { get; set; } //usuário
        [Required]
        [MaxLength(50)]
        public string ClientId { get; set; } //client id (Classe "Client")
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        [Required]
        public string ProtectedTicket { get; set; } //Conterá informações criptografadas necessárias para revalidação do token
    }
}