using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnglishLearning.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage ="Поле {0} не може бути пустим")]
        [Display(Name = "Адрес електронної пошти")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [Display(Name = "Код")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Запам'ятати браузер?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [Display(Name = "Адрес електронної пошти")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [Display(Name = "Адрес електронної пошти")]
        [EmailAddress(ErrorMessage = "Поле {0} не містить допустиму адресу електронної пошти.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [EmailAddress(ErrorMessage = "Поле {0} не містить допустиму адресу електронної пошти.")]
        [Display(Name = "Адрес електронної пошти")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [StringLength(100, ErrorMessage = "Значення {0} повинно містити не менше {2} символів.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження паролю")]
        [Compare("Password", ErrorMessage = "Пароль і його підтвердження не співпадають.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [EmailAddress(ErrorMessage = "Поле {0} не містить допустиму адресу електронної пошти.")]
        [Display(Name = "Адрес електронної пошти")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [StringLength(100, ErrorMessage = "Значення {0} повинно містити не менше {2} символів.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження паролю")]
        [Compare("Password", ErrorMessage = "Пароль і його підтвердження не співпадають.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Поле {0} не може бути пустим")]
        [EmailAddress(ErrorMessage = "Поле {0} не містить допустиму адресу електронної пошти.")]
        [Display(Name = "Пошта")]
        public string Email { get; set; }
    }
}
