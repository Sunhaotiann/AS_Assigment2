using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;

namespace AS_Assigment2.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ResetPasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ResetPasswordInputModel ResetPasswordModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage("/Login");
            }

            // Verify current password
            var passwordCheck = await _userManager.CheckPasswordAsync(user, ResetPasswordModel.CurrentPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError("ResetPasswordModel.CurrentPassword", "Incorrect current password.");
                return Page();
            }

            // Check if the new password is the same as any of the last two passwords
            var passwordHistory = (user as ApplicationUser)?.PasswordHistory ?? new List<string>();
            foreach (var oldPasswordHash in passwordHistory)
            {
                if (_userManager.PasswordHasher.VerifyHashedPassword(user, oldPasswordHash, ResetPasswordModel.NewPassword) == PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("ResetPasswordModel.NewPassword", "You cannot reuse your previous passwords.");
                    return Page();
                }
            }

            // Update password
            var resetResult = await _userManager.ChangePasswordAsync(user, ResetPasswordModel.CurrentPassword, ResetPasswordModel.NewPassword);

            if (resetResult.Succeeded)
            {
                // Update password history
                passwordHistory.Add(_userManager.PasswordHasher.HashPassword(user, ResetPasswordModel.NewPassword));
                if (passwordHistory.Count > 2)
                {
                    passwordHistory.RemoveAt(0); // Keep only the last two passwords
                }

                (user as ApplicationUser).PasswordHistory = passwordHistory;
                await _userManager.UpdateAsync(user);

                TempData["SuccessMessage"] = "Password reset successfully!";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToPage("/Index");
            }

            foreach (var error in resetResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }

    public class ResetPasswordInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
