using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AS_Assigment2.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ChangePasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public ChangePasswordInputModel ChangePasswordInput { get; set; }

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

            // Validate new password is not the same as the current password
            var isSamePassword = await _userManager.CheckPasswordAsync(user, ChangePasswordInput.NewPassword);
            if (isSamePassword)
            {
                ModelState.AddModelError("ChangePasswordInput.NewPassword", "The new password cannot be the same as the current password.");
                return Page();
            }

            // Validate password strength
            string passwordStrength = GetPasswordStrength(ChangePasswordInput.NewPassword);
            if (passwordStrength == "Weak")
            {
                ModelState.AddModelError("ChangePasswordInput.NewPassword", "Your new password is too weak. Try adding more characters, numbers, uppercase, and special symbols.");
                return Page();
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(
                user,
                ChangePasswordInput.CurrentPassword,
                ChangePasswordInput.NewPassword
            );

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToPage("/Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private string GetPasswordStrength(string password)
        {
            int score = 0;

            if (password.Length >= 12) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"\d")) score++;
            if (Regex.IsMatch(password, @"[\W_]")) score++;

            return score switch
            {
                >= 5 => "Strong",
                >= 3 => "Moderate",
                _ => "Weak"
            };
        }
    }

    public class ChangePasswordInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The new password must be at least 8 characters long.")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The confirmation password does not match the new password.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
