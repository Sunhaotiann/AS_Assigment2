using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace AS_Assigment2.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for email: {Email}", Email);
                return Page();
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    _logger.LogWarning("No user found with email: {Email}", Email);
                    TempData["SuccessMessage"] = "If the email is registered, a reset link will be sent.";
                    return Page();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Page(
                    "/ResetPassword",
                    null,
                    new { userId = user.Id, token = token },
                    Request.Scheme);

                // Log the generated reset link for debugging purposes
                _logger.LogInformation("Password reset process initiated for email {Email}.", Email);


                // Send email
                await EmailHelper.SendEmailAsync(
                    Email,
                    "Reset Password",
                    $"Please reset your password by clicking here: <a href='{resetLink}'>Reset Password</a>");

                _logger.LogInformation("Password reset email sent to {Email}", Email);

                TempData["SuccessMessage"] = "If the email is registered, a reset link will be sent.";
                return Page();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the forgot password request for email: {Email}", Email);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return Page();
            }
        }
    }
}
