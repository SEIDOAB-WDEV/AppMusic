using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Models;
using AppMusicRazor.SeidoHelpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppMusicRazor.Pages
{
	public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;

        #region Injected by ASP.NET Core Identity
        private readonly SignInManager<csUser> _signInManager;
        private readonly UserManager<csUser> _userManager;
        private readonly IUserStore<csUser> _userStore;
        private readonly IUserEmailStore<csUser> _emailStore;
        private readonly IEmailSender _emailSender;
        #endregion

        [BindProperty]
        public csUserIM UserIM { get; set; }

        //For Validation and Identity Errors
        public reModelValidationResult ValidationResult { get; set; } = new reModelValidationResult(false, null, null);


        #region reguired by ASP.NET Core Identity
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        #endregion

        public RegisterModel(
            ILogger<RegisterModel> logger,
            UserManager<csUser> userManager,
            IUserStore<csUser> userStore,
            SignInManager<csUser> signInManager,
            IEmailSender emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            #region reguired by ASP.NET Core Identity
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            UserIM = new csUserIM();
            #endregion
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            if (!ModelState.IsValidPartially(out reModelValidationResult validationResult))
            {
                ValidationResult = validationResult;
                return Page();
            }

            //Continute to create a user with ASP.NET Core Identity
            var user = UserIM.UpdateModel(new csUser());

            await _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, UserIM.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = user.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            else
            {
                //Here I simple use Validation Error to show error from Identity.
                //Could be a seperate modal or separate page
                var identityErrors = result.Errors.Select(e => e.Description).ToList();
                ValidationResult = new reModelValidationResult(true, identityErrors, null);
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

        #region reguired by ASP.NET Core Identity
        private IUserEmailStore<csUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<csUser>)_userStore;
        }
        #endregion

        #region Input Model
        //InputModel (IM) is locally declared classes that contains ONLY the properties of the Model
        //that are bound to the <form> tag
        //EVERY property must be bound to an <input> tag in the <form>
        //These classes are in center of ModelBinding and Validation
        public class csUserIM
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            //The Basic IM methods
            public csUser UpdateModel(csUser model)
            {
                model.FirstName = this.FirstName;
                model.LastName = this.LastName;
                model.Email = this.Email;
                return model;
            }

            public csUserIM() { }
            public csUserIM(csUserIM original)
            {
                FirstName = original.FirstName;
                LastName = original.LastName;
                Email = original.Email;
            }
            public csUserIM(csUser model)
            {
                FirstName = model.FirstName;
                LastName = model.LastName;
                Email = model.Email;
            }
        }
        #endregion
    }
}
