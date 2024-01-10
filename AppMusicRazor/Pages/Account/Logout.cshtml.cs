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
using static AppMusicRazor.Pages.Account.LoginModel;

namespace AppMusicRazor.Pages.Account
{
	public class LogoutModel : PageModel
    {
        private readonly SignInManager<csUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<csUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            await _signInManager.SignOutAsync();
            return LocalRedirect(returnUrl);
        }
    }
}
