//Classe POCO contendo propriedades necessárias para autenticação do usuário
using System.ComponentModel.DataAnnotations;

namespace TemplateMultiTenant.Auth.ViewModel
{
    public class UserModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve conter {2} caracteres no mínimo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "O password e confirmação do password não estão iguais")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "e-mail")]       
        [EmailAddress] 
        public string Email { get; set; }

        [Required]
        public string ClientId { get; set; }
    }


}