using System;
using System.Threading.Tasks;
using Burg_Storage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Burg_Storage.Controllers
{
    /// <summary>
    /// Handles authentication endpoints: Login (GET/POST) and Logout (POST).
    /// Uses LoginViewModel from Burg_Storage.Models.
    /// </summary>
    [AllowAnonymous]
    public sealed class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// DI constructor for the controller.
        /// </summary>
        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Shows the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        /// <summary>
        /// Attempts to sign in with the provided credentials.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Try to resolve the user by email or username
            ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(model.UserNameOrEmail))
            {
                if (model.UserNameOrEmail.Contains("@", StringComparison.Ordinal))
                {
                    user = await _userManager.FindByEmailAsync(model.UserNameOrEmail.Trim());
                }
                if (user is null)
                {
                    user = await _userManager.FindByNameAsync(model.UserNameOrEmail.Trim());
                }
            }

            if (user is null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account temporarily locked.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(model);
        }

        /// <summary>
        /// Logs out the current user and redirects to home.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
