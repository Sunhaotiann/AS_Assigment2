using AS_Assigment2.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace AS_Assigment2.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                TempData["SuccessMessage"] = "If the email exists in our system, a password reset link will be sent.";
                return Page();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { userId = user.Id, token = token },
                protocol: Request.Scheme);

            // Send the email with the reset link (replace with your email logic)
            await SendResetEmailAsync(Email, resetLink);

            TempData["SuccessMessage"] = "If the email exists in our system, a password reset link will be sent.";
            return Page();
        }

        private Task SendResetEmailAsync(string email, string resetLink)
        {
            // Replace this with actual email sending logic
            System.Console.WriteLine($"Send reset link to: {email}");
            System.Console.WriteLine($"Reset Link: {resetLink}");
            return Task.CompletedTask;
        }
    }
}
