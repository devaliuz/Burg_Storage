using System.ComponentModel.DataAnnotations;

namespace Burg_Storage.Models
{
    /// <summary>
    /// ViewModel for the login form.
    /// </summary>
    public sealed class LoginViewModel
    {
        /// <summary>
        /// Username or email address entered by the user.
        /// Accepts either to keep the UX simple.
        /// </summary>
        [Required]
        [Display(Name = "Username or Email")]
        [MaxLength(256)]
        public string UserNameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// The user's password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MaxLength(256)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Whether the authentication cookie should persist across browser sessions.
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// The URL to redirect to after a successful login.
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}
