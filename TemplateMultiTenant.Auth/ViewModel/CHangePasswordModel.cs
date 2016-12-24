//Classe POCO contendo propriedades necessárias para autenticação do usuário
using TemplateMultiTenant.Auth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TemplateMultiTenant.Auth.ViewModel
{
    public class ChangePasswordModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve conter {2} caracteres no mínimo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

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