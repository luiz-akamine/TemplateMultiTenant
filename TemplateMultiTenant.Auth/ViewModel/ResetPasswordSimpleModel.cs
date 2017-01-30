//Classe POCO contendo propriedades necessárias para autenticação do usuário
using System.ComponentModel.DataAnnotations;

namespace TemplateMultiTenant.Auth.ViewModel
{
    public class ResetPasswordSimpleModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "e-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public int CNPJ { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve conter {2} caracteres no mínimo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "O password e confirmação do password não estão iguais")]
        public string ConfirmNewPassword { get; set; }
    }


}