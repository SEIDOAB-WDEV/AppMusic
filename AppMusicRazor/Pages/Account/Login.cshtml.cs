using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using AppMusicRazor.SeidoHelpers;
using static AppMusicRazor.Pages.RegisterModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AppMusicRazor.Pages.Account
{
	public class LoginModel : PageModel
    {
        private readonly SignInManager<csUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<csUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public csLoginIM LoginIM { get; set; }

        //For Validation and Identity Errors
        public reModelValidationResult ValidationResult { get; set; } = new reModelValidationResult(false, null, null);

        //public string ReturnUrl { get; set; }

        public class csLoginIM
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        public void OnGetAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            //ReturnUrl = returnUrl;
            LoginIM = new csLoginIM();

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult))
            {
                ValidationResult = validationResult;
                return Page();
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(LoginIM.Email, LoginIM.Password, LoginIM.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect("~/");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = LoginIM.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }

            //Failed Login
            ValidationResult = new reModelValidationResult(true,
                    new List<string>() { "Invalid login attempt." }, null) ;
            return Page();
        }
    }
}
