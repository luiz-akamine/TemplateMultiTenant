﻿//Classe POCO contendo propriedades necessárias para autenticação do usuário
using TemplateMultiTenant.Auth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TemplateMultiTenant.Auth.ViewModel
{
    public class UserModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O {0} deve conter {2} no mínimo.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "O password e confirmação do password não estão iguais")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ClientId { get; set; }
    }


}